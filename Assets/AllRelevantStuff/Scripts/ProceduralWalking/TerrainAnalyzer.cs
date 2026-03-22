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
        private float _upwardOffset = 3f;

        public Vector2 CheckAtPosition(Vector2 worldPosition)
        {
            RaycastHit2D hit = Physics2D.CircleCast(worldPosition + Vector2.up * _upwardOffset,
                _footWidth / 2, Vector2.down, _maxRaycastDistance, _groundLayer);

            if (hit.collider != null)
            {
                return hit.point;
            }

            return worldPosition;
        }

        public TerrainReport CheckTerrain(Vector2 origin, float bodyRadius, FacingDirection facingDirection)
        {
            Vector2 leftOrigin = new Vector2(origin.x - bodyRadius, origin.y - bodyRadius);
            Vector2 rightOrigin = new Vector2(origin.x + bodyRadius, origin.y - bodyRadius);
            Vector2 middleOrigin = new Vector2(origin.x, origin.y - bodyRadius);

            RaycastHit2D leftRaycast = Physics2D.Raycast(leftOrigin, Vector2.down, _maxRaycastDistance, _groundLayer);
            RaycastHit2D rightRaycast = Physics2D.Raycast(rightOrigin, Vector2.down, _maxRaycastDistance, _groundLayer);
            RaycastHit2D middleRaycast = Physics2D.Raycast(middleOrigin, Vector2.down, _maxRaycastDistance, _groundLayer);

            RaycastHit2D frontRaycast =  facingDirection == FacingDirection.Right ? rightRaycast : leftRaycast;
            RaycastHit2D backRaycast =  facingDirection == FacingDirection.Right ? leftRaycast : rightRaycast;

            float frontRaycastDistance = frontRaycast.distance;
            float backRaycastDistance = backRaycast.distance;

            Vector2 averageNormal = (frontRaycast.normal + backRaycast.normal).normalized;

            float slopeAngle = Vector2.Angle(Vector2.up, averageNormal);
            float expectedDifference = (bodyRadius * 2f) * Mathf.Tan(slopeAngle * Mathf.Deg2Rad);
            float heightDifference = Mathf.Abs(frontRaycastDistance - backRaycastDistance);
            
            bool isSteepObstacle = (heightDifference - expectedDifference) > _minObstacleThreshold;
            bool isWithinMaxHeight = heightDifference <= _maxObstacleThreshold;

            bool isSlopeTooSteep = slopeAngle > _maxSlopeAngle;
            bool isMovementBlocked = (!isWithinMaxHeight || isSlopeTooSteep) && (frontRaycastDistance < backRaycastDistance);

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

        public JumpReport CheckJump(Vector2 origin, float standingHeight, float bodyRadius, float velocityY)
        {
            Vector2 middleOrigin = new Vector2(origin.x, origin.y - bodyRadius);
            RaycastHit2D middleRaycast = Physics2D.Raycast(middleOrigin, Vector2.down, _maxRaycastDistance, _groundLayer);

            int framesForPreparation = 8;

            float fallPerFrame = Mathf.Abs(velocityY) * Time.fixedDeltaTime;
            if (fallPerFrame < 0.001f)
            {
                fallPerFrame = 0.001f;
            }

            JumpReport jumpReport = new JumpReport();
            if(middleRaycast.collider != null)
            {
                int framesBeforeLanding = (int)(middleRaycast.distance / fallPerFrame);

                bool isGrounded = middleRaycast.distance <= standingHeight;
                bool ShouldPrepareForLanding = !isGrounded && framesBeforeLanding <= framesForPreparation;

                jumpReport.IsGrounded = isGrounded;
                jumpReport.ShouldPrepareForLanding = ShouldPrepareForLanding;
            }

            return jumpReport;
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
    }
}
