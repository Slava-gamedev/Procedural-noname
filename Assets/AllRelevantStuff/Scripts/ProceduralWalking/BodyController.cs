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
        [SerializeField] private GroundDetector _groundDetector;
        [SerializeField] private CharacterMovementController _movementController;

        [SerializeField] private float _minObstacleThreshold = 0.2f;
        [SerializeField] private float _maxObstacleThreshold = 1f;

        private float _fullLegLength;
        private float _bodyRadius;
        private float _cachedHitDistance;
        private Vector2 _cachedHitPoint;


        private void Start()
        {
            _fullLegLength = _rightLeg.FullLegLength;
            _bodyRadius = transform.localScale.x / 2;

            RaycastHit2D hit = _groundDetector.ProjectBodyOnTheGround(transform.position, _bodyRadius);
            _cachedHitDistance = hit.distance;
        }

        private void FixedUpdate()
        {
            RaycastHit2D hit = _groundDetector.ProjectBodyOnTheGround(transform.position, _bodyRadius);

            if (hit.collider == null)
            {
                return;
            }

            if (hit.distance > _standingHeight)
            {
                return;
            }

            float finalPerceivedDistance = hit.distance;
            float heightDifference = Mathf.Abs(hit.distance - _cachedHitDistance);

            if (heightDifference >= _minObstacleThreshold && heightDifference <= _maxObstacleThreshold)
            {
                _cachedHitPoint = _cachedHitPoint == Vector2.zero ? hit.point : _cachedHitPoint;

                bool isFacingRight = _movementController.FacingDirection == FacingDirection.Right;
                float startX = isFacingRight ? _cachedHitPoint.x - _bodyRadius : _cachedHitPoint.x + _bodyRadius; 
                float endX = isFacingRight ? _cachedHitPoint.x + _bodyRadius : _cachedHitPoint.x - _bodyRadius; 
                float currentX = transform.position.x;

                float interpolationParameter = Mathf.InverseLerp(startX, endX, currentX);
                //float ascendParameter = Mathf.InverseLerp(0.5f, 1f, interpolationParameter);

                finalPerceivedDistance = Mathf.Lerp(_cachedHitDistance, hit.distance, interpolationParameter);
            }
            else
            {
                _cachedHitDistance = hit.distance;
                _cachedHitPoint = Vector2.zero;
            }

            float verticalOffset = CalculateVerticalOffset();
            float finalStandingHeight = _standingHeight + verticalOffset;

            ApplyForceToRigidbody(finalPerceivedDistance, finalStandingHeight);
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
            RaycastHit2D hit = _groundDetector.ProjectBodyOnTheGround(transform.position, _bodyRadius);
            Gizmos.color = Color.yellow;
            Vector3 end = hit.point + (hit.normal * _maxObstacleThreshold);
            Gizmos.DrawLine(hit.point, end);
        }
    }
}
