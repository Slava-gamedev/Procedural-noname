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
        private float _startInterpolationX;
        private float _endInterpolationX;
        private FacingDirection _direction => _movementController.FacingDirection;

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
            CacheReport(terrainReport);

            if (terrainReport.ShouldAscend)
            {
                return GetOffsetFromAscending();
            }
            else if (terrainReport.ShouldDescend)
            {
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
                _startedAscending = true;
                CalculateInterpolationPoints(reportValue);
            }

            float interpolationParameter = Mathf.InverseLerp(_startInterpolationX, _endInterpolationX, currentX);
            resultingOffset = interpolationParameter * heightDelta;
           
            return resultingOffset;
        }

        private float GetOffsetFromDescending()
        {
            float resultingOffset = 0;
            TerrainReport reportValue = _cachedReport.Value;
            float heightDelta = reportValue.BodyDistance - reportValue.FrontDistance;
            float currentX = transform.position.x;

            float t = Mathf.InverseLerp(_startInterpolationX, _endInterpolationX, currentX);
            
            //float curvedT = t * t * (3f - 2f * t);
            //float curvedT = t * t;
            resultingOffset = t * heightDelta;

            return resultingOffset;
        }

        private void CacheReport(TerrainReport terrainReport)
        {
            if (_cachedReport == null)
            {
                _cachedReport = terrainReport;
                _startedAscending = false;
                CalculateInterpolationPoints(_cachedReport.Value);
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

        private void CalculateInterpolationPoints(TerrainReport terrainReport)
        {
            float directionXModifier = _direction == FacingDirection.Right ? 1 : -1;
            float currentX = transform.position.x;

            if (terrainReport.ShouldAscend && _startedAscending)
            {
                _startInterpolationX = currentX;
                _endInterpolationX = currentX + (directionXModifier * _bodyRadius);
            }
            else if (terrainReport.ShouldDescend)
            {
                _startInterpolationX = terrainReport.FrontHitPoint.x - (directionXModifier * _bodyRadius / 2f);
                _endInterpolationX = terrainReport.FrontHitPoint.x + (directionXModifier * _bodyRadius / 2f);
            }
        }

        private void OnDrawGizmos()
        {
            if (_cachedReport != null)
            {
                TerrainReport reportValue = _cachedReport.Value;

                Gizmos.color = Color.red;
                Vector2 start = new Vector2(_startInterpolationX, transform.position.y);
                Vector2 end = new Vector2(_endInterpolationX, transform.position.y);

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
