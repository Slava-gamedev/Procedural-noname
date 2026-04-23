using UnityEngine;

namespace CharacterMovement
{
    public class TerrainAnalyzer : MonoBehaviour
    {
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private float _footWidth;
        [SerializeField] private float _minObstacleThreshold = 0.1f;
        [SerializeField] private float _maxObstacleThreshold = 0.91f;
        [SerializeField] private float _maxSlopeAngle = 60f;
        private float _maxRaycastDistance = 30f;
        private float _minRaycastDistance = 0.1f;

        public Vector2 CheckGroundBelowPosition(Vector2 worldPosition)
        {
            RaycastHit2D hit = Physics2D.CircleCast(worldPosition,
                _footWidth / 2, Vector2.down, _maxRaycastDistance, _groundLayer);

            if (hit.collider != null)
            {
                return hit.point;
            }

            return worldPosition;
        }

        public Vector2 GetStepTarget(Vector2 pelvisPosition, float stepLength, int directionX)
        {
            Vector2 currentGroundPosition = CheckGroundBelowPosition(pelvisPosition);
            Vector2 raycastOrigin = currentGroundPosition + new Vector2(0, _maxObstacleThreshold);
            Vector2 secondRaycastOrigin;
            Vector2 result;

            RaycastHit2D raycastHit = Physics2D.Raycast(raycastOrigin, new Vector2(directionX,0), stepLength + _footWidth / 2, _groundLayer);

            if (raycastHit.collider != null)
            {
                float originX = raycastHit.point.x - directionX * _footWidth;
                secondRaycastOrigin = new Vector2(originX, raycastHit.point.y);

                result = CheckGroundBelowPosition(secondRaycastOrigin);
                return result;
            }

            secondRaycastOrigin = raycastOrigin + new Vector2(directionX, 0) * stepLength;

            result = CheckGroundBelowPosition(secondRaycastOrigin);
            return result;
        }

        public TerrainReport CheckTerrain(Vector2 origin, float bodyRadius, FacingDirection facingDirection)
        {
            Vector2 leftOrigin = new Vector2(origin.x - bodyRadius, origin.y - bodyRadius);
            Vector2 rightOrigin = new Vector2(origin.x + bodyRadius, origin.y - bodyRadius);
            Vector2 middleOrigin = new Vector2(origin.x, origin.y - bodyRadius);

            RaycastHit2D leftRaycast = Physics2D.Raycast(leftOrigin, Vector2.down, _maxRaycastDistance, _groundLayer);
            RaycastHit2D rightRaycast = Physics2D.Raycast(rightOrigin, Vector2.down, _maxRaycastDistance, _groundLayer);
            RaycastHit2D middleRaycast = Physics2D.Raycast(middleOrigin, Vector2.down, _maxRaycastDistance, _groundLayer);

            int direction = facingDirection == FacingDirection.Right ? 1 : -1;
            RaycastHit2D obsacleRaycast = Physics2D.CircleCast(origin, bodyRadius, Vector2.right * direction, _minRaycastDistance, _groundLayer);

            RaycastHit2D frontRaycast =  facingDirection == FacingDirection.Right ? rightRaycast : leftRaycast;
            RaycastHit2D backRaycast =  facingDirection == FacingDirection.Right ? leftRaycast : rightRaycast;

            float frontRaycastDistance = frontRaycast.distance;
            float backRaycastDistance = backRaycast.distance;

            Vector2 averageNormal = (frontRaycast.normal + backRaycast.normal).normalized;

            float slopeAngle = Vector2.Angle(Vector2.up, averageNormal);
            float expectedDifference = (bodyRadius * 2f) * Mathf.Tan(slopeAngle * Mathf.Deg2Rad);
            float heightDifference = Mathf.Abs(frontRaycastDistance - backRaycastDistance);
            
            bool isObstacleAhead = obsacleRaycast.collider != null;

            bool isSteepObstacle = (heightDifference - expectedDifference) > _minObstacleThreshold;
            bool isWithinMaxHeight = heightDifference <= _maxObstacleThreshold;

            bool isSlopeTooSteep = slopeAngle > _maxSlopeAngle;
            bool isMovementBlocked = (!isWithinMaxHeight || isSlopeTooSteep) && (isObstacleAhead);

            bool shouldAscend = isSteepObstacle && isWithinMaxHeight && frontRaycastDistance < backRaycastDistance;
            bool shouldDescend = isSteepObstacle && isWithinMaxHeight && frontRaycastDistance > backRaycastDistance;

            TerrainReport report = new TerrainReport();

            report.IsMovementBlocked = isMovementBlocked;
            report.ShouldDescend = shouldDescend;
            report.ShouldAscend = shouldAscend;
            report.FrontDistance = frontRaycastDistance;
            report.BodyDistance = middleRaycast.distance;
            report.MiddleHitPoint = middleRaycast.point;
            report.FrontHitPoint = frontRaycast.point;
            report.SurfaceNormal = averageNormal;
            return report;
        }

        public JumpReport CheckJump(Vector2 origin, float standingHeight, float bodyRadius, Rigidbody2D rigidbody)
        {
            Vector2 middleOrigin = new Vector2(origin.x, origin.y - bodyRadius);
            RaycastHit2D middleRaycast = Physics2D.Raycast(middleOrigin, Vector2.down, _maxRaycastDistance, _groundLayer);

            JumpReport jumpReport = new JumpReport();
            if(middleRaycast.collider != null)
            {
                float currentDistance = middleRaycast.distance;
                jumpReport.IsGrounded = currentDistance <= standingHeight;

                float distanceBeforeLanding = currentDistance - standingHeight;

                if(distanceBeforeLanding > 0)
                {
                    jumpReport.TimeToLanding = CalculateTimeToLanding(distanceBeforeLanding, rigidbody);
                }

                jumpReport.ShouldPrepareForLanding = !jumpReport.IsGrounded;
            }

            return jumpReport;
        }

        private float CalculateTimeToLanding(float distanceBeforeLanding, Rigidbody2D rigidbody)
        {
            float startingVelocity = Mathf.Abs(rigidbody.linearVelocityY);
            float acceleration = Mathf.Abs(Physics2D.gravity.y * rigidbody.gravityScale);

            float discriminant = startingVelocity * startingVelocity + 2f * acceleration * distanceBeforeLanding;

            float timeToLanding = (Mathf.Sqrt(discriminant) - startingVelocity) / acceleration;
            return timeToLanding;
        }
    }

    public struct TerrainReport
    {
        public bool ShouldAscend;
        public bool ShouldDescend;
        public bool IsMovementBlocked;
        public float BodyDistance;
        public float FrontDistance;
        public Vector2 FrontHitPoint;
        public Vector2 MiddleHitPoint;
        public Vector2 SurfaceNormal;
    }

    public struct JumpReport
    {
        public bool IsGrounded;
        public bool ShouldPrepareForLanding;
        public float TimeToLanding;
    }
}
