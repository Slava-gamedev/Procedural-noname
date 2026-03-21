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
        private bool _startedAscending;

        private void Start()
        {
            _fullLegLength = _rightLeg.FullLegLength;
            _bodyRadius = transform.localScale.x / 2;
        }

        private void FixedUpdate()
        {
            TerrainReport report = _terrainAnalyzer.CheckTerrain(transform.position,
                _bodyRadius, _movementController.FacingDirection);

            if (report.BodyDistance > _standingHeight && _cachedReport == null)
            {
                return;
            }

            float currentDistanceToGround = report.BodyDistance;

            float terrainOffset = CalculateVerticalOffsetFromTerrain(report);

            float stepLengthOffset = CalculateVerticalOffsetFromStepLength();

            float heightCorrection = 0;

            if (_cachedReport != null)
            {
                heightCorrection = _cachedReport.Value.MiddleHitPoint.y - report.MiddleHitPoint.y;
            }

            float finalStandingHeight = _standingHeight + stepLengthOffset + terrainOffset + heightCorrection;

            ApplyForceToRigidbody(currentDistanceToGround, finalStandingHeight);
        }

        private float CalculateVerticalOffsetFromTerrain(TerrainReport terrainReport)
        {
            if (terrainReport.ShouldAscend)
            {
                CacheReport(terrainReport);
                return GetOffsetFromAscending();
            }
            else if (terrainReport.ShouldDescend)
            {
                CacheReport(terrainReport);
                return GetOffsetFromDescending();
            }
            else
            {
                ClearCachedReport();
                return 0;
            }
        }

        private float GetOffsetFromAscending()
        {
            float resultingOffset = 0;

            TerrainReport reportValue = _cachedReport.Value;

            Vector2 hitPoint = reportValue.FrontHitPoint;
            float currentX = transform.position.x;

            float heightDelta = reportValue.BodyDistance - reportValue.FrontDistance;

            if (!OneLegOnNewHeight(hitPoint.y))
            {
                return resultingOffset;
            }

            if (!_startedAscending)
            {
                reportValue.StartInterpolationX = currentX;
                _cachedReport = reportValue;
                _startedAscending = true;
            }

            float interpolationParameter = Mathf.InverseLerp(reportValue.StartInterpolationX, reportValue.EndInterpolationX, currentX);
            resultingOffset = Mathf.Lerp(0, heightDelta, interpolationParameter);
            return resultingOffset;
        }

        private float GetOffsetFromDescending()
        {
            float resultingOffset = 0;
            TerrainReport reportValue = _cachedReport.Value;
            float heightDelta = reportValue.BodyDistance - reportValue.FrontDistance;
            float currentX = transform.position.x;

            float interpolationParameter = Mathf.InverseLerp(reportValue.StartInterpolationX, reportValue.EndInterpolationX, currentX);
            resultingOffset = Mathf.Lerp(0, heightDelta, interpolationParameter);

            return resultingOffset;
        }

        private void CacheReport(TerrainReport terrainReport)
        {
            if (_cachedReport == null)
            {
                _cachedReport = terrainReport;
                _startedAscending = false;
            }
        }

        private void ClearCachedReport()
        {
            _startedAscending = false;
            _cachedReport = null;
        }

        private bool OneLegOnNewHeight(float newHeight)
        {
            float yDifferenceThreshold = 0.05f;

            Vector2 leftLegFoot = _leftLeg.TargetFootPosition;
            Vector2 rightLegFoot = _rightLeg.TargetFootPosition;

            float yDifferenceForLeftLeg = Mathf.Abs(newHeight - leftLegFoot.y);
            float yDifferenceForRightLeg = Mathf.Abs(newHeight - rightLegFoot.y);

            if (yDifferenceForLeftLeg < yDifferenceThreshold && !_leftLeg.IsMoving
                || yDifferenceForRightLeg < yDifferenceThreshold && !_rightLeg.IsMoving)
            {
                return true;
            }

            return false;
        }

        private void ApplyForceToRigidbody(float currentDistanceToGround, float targetStandingHeight)
        {
            float verticalVelocity = _rigidbody.linearVelocityY;
            float force = (targetStandingHeight - currentDistanceToGround) * _stiffness - (verticalVelocity * _damping);

            force = force * _rigidbody.mass;

            _rigidbody.AddForceY(force, ForceMode2D.Force);
        }

        private float CalculateVerticalOffsetFromStepLength()
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
            if (_cachedReport != null)
            {
                TerrainReport reportValue = _cachedReport.Value;

                Gizmos.color = Color.red;
                Vector2 start = new Vector2(reportValue.StartInterpolationX, transform.position.y);
                Vector2 end = new Vector2(reportValue.EndInterpolationX, transform.position.y);

                Gizmos.DrawLine(start, start + Vector2.down);
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(end, end + Vector2.down);

                Gizmos.color = Color.green;
                Vector2 middle = transform.position;
                Gizmos.DrawLine(middle, middle + Vector2.down);

            }
        }
    }
}
