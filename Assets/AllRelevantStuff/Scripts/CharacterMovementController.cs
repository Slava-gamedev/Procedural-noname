using UnityEngine;

namespace CharacterMovement
{
    public class CharacterMovementController : MonoBehaviour
    {
        [SerializeField] private float _maxMovementSpeed;
        [SerializeField] private float _acceleration;
        [SerializeField] private float _deceleration;
        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private float _cycleFrequency = 1f;

        private Vector2 _currentMovementDirection;

        public float LocomotionCycle {  get; private set; }
        public Vector2 MovementDirection => _currentMovementDirection;
        public float MaxSpeed => _maxMovementSpeed;
        public float CurrentAbsoluteSpeed { get; private set; }
        public float CurrentSignedSpeed { get; private set; }
        public float CycleFrequency => _cycleFrequency;

        void Update()
        {
            CheckForInput();
            UpdateLocomotionCycle();
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void CheckForInput()
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                _currentMovementDirection = Vector2.right;
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                _currentMovementDirection = Vector2.left;
            }
            else
            {
                _currentMovementDirection = Vector2.zero;
            }
        }

        private void Move()
        {
            float targetSpeed = _currentMovementDirection.x * _maxMovementSpeed;

            float accelRate = (_currentMovementDirection.x != 0)
                ? _acceleration
                : _deceleration;

            float newVelocityX = Mathf.MoveTowards(
                _rigidbody.linearVelocityX,
                targetSpeed,
                accelRate * Time.fixedDeltaTime
            );

            _rigidbody.linearVelocity = new Vector2(newVelocityX, _rigidbody.linearVelocityY);

            CurrentAbsoluteSpeed = Mathf.Abs(_rigidbody.linearVelocityX);
            CurrentSignedSpeed = _rigidbody.linearVelocityX;
        }

        private void UpdateLocomotionCycle()
        {
            float deltaPhase = CurrentAbsoluteSpeed * _cycleFrequency * Time.deltaTime;
            LocomotionCycle += deltaPhase;
            LocomotionCycle = Mathf.Repeat(LocomotionCycle, 1);
        }
    }
}
