using UnityEngine;

namespace CharacterMovement
{
    public class BodyController : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private float _standingHeight;
        [SerializeField] private float _damping;
        [SerializeField] private float _stiffness;
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private float _raycastLength;

        [SerializeField] private LegIKController _rightLeg;
        [SerializeField] private LegIKController _leftLeg;


        private float _fullLegLength;
        private float _bodyRadius;

        private void Start()
        {
            _fullLegLength = _rightLeg.FullLegLength;
            _bodyRadius = transform.localScale.x / 2;
        }

        private void FixedUpdate()
        {
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, _bodyRadius, Vector2.down, _raycastLength, _groundLayer);

            if (hit.collider == null)
            {
                return;
            }

            if(hit.distance <= _standingHeight)
            {
                float verticalOffset = CalculateVerticalOffset();
                float finalStandingHeight = _standingHeight + verticalOffset;

                ApplyForceToRigidbody(hit.distance, finalStandingHeight);
            }
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
            float currentStepWidth = Mathf.Abs(_rightLeg.CurrentFootPosition.x - _leftLeg.CurrentFootPosition.x); 
            float halfWidth = currentStepWidth / 2;

            halfWidth = Mathf.Clamp(halfWidth, 0, _fullLegLength);
            float currentHeight = Mathf.Sqrt(_fullLegLength * _fullLegLength - (halfWidth * halfWidth));
            float verticalOffset = currentHeight - _fullLegLength;

            return verticalOffset;
        }
    }
}
