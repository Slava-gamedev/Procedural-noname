using UnityEngine;

namespace CharacterMovement
{
    public class BodyController : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private float _standingHeight;
        [SerializeField] private float _damping;
        [SerializeField] private float _stiffness;

        [SerializeField] private LegIKController _rightLeg;
        [SerializeField] private LegIKController _leftLeg;
        [SerializeField] private TerrainAnalyzer _terrainAnalyzer;
        [SerializeField] private CharacterMovementController _movementController;

        private float _fullLegLength;
        private float _bodyRadius;
        private TerrainReport? _cachedReport;

        private void Start()
        {
            _fullLegLength = _rightLeg.FullLegLength;
            _bodyRadius = transform.localScale.x / 2;
        }

        private void FixedUpdate()
        {
            TerrainReport report = _terrainAnalyzer.CheckTerrain(transform.position,
                _bodyRadius, _movementController.FacingDirection);

            if(report.BodyDistance > _standingHeight)
            {
                return;
            }

            float finalPerceivedDistance = report.FrontDistance;

            if (report.ShouldDescend || report.ShouldAscend)
            {
                _cachedReport = _cachedReport == null ? report : _cachedReport;
                TerrainReport reportValue = _cachedReport.Value;

                Vector2 hitPoint = reportValue.FrontHitPoint;
                float interpolationParameter = 0;
                float currentX = transform.position.x;

                if (OneLegOnNewHeight(hitPoint.y) && reportValue.ShouldAscend)
                {
                    interpolationParameter = Mathf.InverseLerp(reportValue.StartInterpolationX, reportValue.EndInterpolationX, currentX);
                }
                else if (report.ShouldDescend)
                {
                    interpolationParameter = Mathf.InverseLerp(reportValue.StartInterpolationX, reportValue.EndInterpolationX, currentX);
                }

                finalPerceivedDistance = Mathf.Lerp(reportValue.BodyDistance, reportValue.FrontDistance, interpolationParameter);
            }
            else
            {
                _cachedReport = null;
            }

            float verticalOffset = CalculateVerticalOffset();
            float finalStandingHeight = _standingHeight + verticalOffset;

            ApplyForceToRigidbody(finalPerceivedDistance, finalStandingHeight);
        }

        private bool OneLegOnNewHeight(float newHeight)
        {
            float yDifferenceThreshold = 0.05f;

            Vector2 leftLegFoot = _leftLeg.TargetFootPosition;
            Vector2 rightLegFoot = _rightLeg.TargetFootPosition;

            float yDifferenceForLeftLeg = Mathf.Abs(newHeight - leftLegFoot.y);
            float yDifferenceForRightLeg = Mathf.Abs(newHeight - rightLegFoot.y);

            if(yDifferenceForLeftLeg < yDifferenceThreshold && !_leftLeg.IsMoving
                || yDifferenceForRightLeg < yDifferenceThreshold && !_rightLeg.IsMoving)
            {
                return true;
            }

            return false;
        }

        private void ApplyForceToRigidbody(float currentDistance, float targetDistance)
        {
            float verticalVelocity = _rigidbody.linearVelocityY;
            float force = (targetDistance - currentDistance) * _stiffness - (verticalVelocity * _damping);
            force = force * _rigidbody.mass;

            _rigidbody.AddForceY(force, ForceMode2D.Force);
        }

        private float CalculateVerticalOffset()
        {
            float currentStepWidth = Vector2.Distance(_rightLeg.CurrentFootPosition, _leftLeg.CurrentFootPosition);
            float halfWidth = currentStepWidth / 2;

            halfWidth = Mathf.Clamp(halfWidth, 0, _fullLegLength);
            float currentHeight = Mathf.Sqrt(_fullLegLength * _fullLegLength - (halfWidth * halfWidth));
            float verticalOffset = currentHeight - _fullLegLength;

            return verticalOffset;
        }

        private void OnDrawGizmos()
        {
            if(_cachedReport != null)
            {
                TerrainReport reportValue = _cachedReport.Value;

                Gizmos.color = Color.red;
                Vector2 start = new Vector2(reportValue.StartInterpolationX, transform.position.y);
                Vector2 end = new Vector2(reportValue.EndInterpolationX, transform.position.y);

                Gizmos.DrawLine(start, start + Vector2.down);
                Gizmos.DrawLine(end, end + Vector2.down);

                Gizmos.color = Color.green;
                Vector2 middle = transform.position;
                Gizmos.DrawLine(middle, middle + Vector2.down);

            }
        }
    }
}
