using UnityEngine;

namespace CharacterMovement
{
    public class TerrainAnalyzer : MonoBehaviour
    {
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private float _footWidth;
        [SerializeField] private float _minObstacleThreshold = 0.1f;
        [SerializeField] private float _maxObstacleThreshold = 0.91f;
        private float _maxRaycastDistance = 30f;
        private float _upwardOffset = 3f;


        public float GetGroundHeightAtPosition(Vector2 worldPosition)
        {
            RaycastHit2D hit = Physics2D.CircleCast(worldPosition + Vector2.up * _upwardOffset,
                _footWidth / 2, Vector2.down, _maxRaycastDistance, _groundLayer);

            if (hit.collider != null)
            {
                return hit.point.y;
            }

            return worldPosition.y;
        }

        public RaycastHit2D ProjectBodyOnTheGround(Vector2 origin, float bodyRadius)
        {
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, bodyRadius, Vector2.down, _maxRaycastDistance, _groundLayer);

            return hit;
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

            float heightDifference = Mathf.Abs(frontRaycastDistance - backRaycastDistance);

            bool isDifferenceAcceptable = heightDifference >= _minObstacleThreshold && heightDifference <= _maxObstacleThreshold;

            float directionXModifier = facingDirection == FacingDirection.Right ? 1 : -1;

            bool shouldAscend = isDifferenceAcceptable && frontRaycastDistance < backRaycastDistance;
            bool shouldDescend = isDifferenceAcceptable && frontRaycastDistance > backRaycastDistance;

            TerrainReport report = new TerrainReport();

            report.ShouldDescend = shouldDescend;
            report.ShouldAscend = shouldAscend;
            report.FrontDistance = frontRaycastDistance;
            report.BodyDistance = middleRaycast.distance;
            report.MiddleHitPoint = middleRaycast.point;
            report.FrontHitPoint = frontRaycast.point;

            if (shouldAscend)
            {
                report.StartInterpolationX = frontRaycast.point.x - (directionXModifier * bodyRadius);
                report.EndInterpolationX = frontRaycast.point.x + (directionXModifier * bodyRadius);
            }
            else if (shouldDescend)
            {
                report.StartInterpolationX = frontRaycast.point.x - (directionXModifier * bodyRadius / 2f);
                report.EndInterpolationX = frontRaycast.point.x + (directionXModifier * bodyRadius / 2f);
            }

            return report;
        }
    }

    public struct TerrainReport
    {
        public bool ShouldAscend;
        public bool ShouldDescend;
        public float BodyDistance;
        public float FrontDistance;
        public Vector2 FrontHitPoint;
        public Vector2 MiddleHitPoint;
        public float StartInterpolationX;
        public float EndInterpolationX;
    }
}
