using UnityEngine;

namespace CharacterMovement
{
    public class CharacterMovementController : MonoBehaviour
    {
        [SerializeField] private float _maxMovementSpeed;
        [SerializeField] private float _acceleration;
        [SerializeField] private float _deceleration;
        [SerializeField] private Rigidbody2D _rigidbody;


        private Vector2 _currentMovementDirection;

        public Vector2 MovementDirection => _currentMovementDirection;
        public float MaxSpeed => _maxMovementSpeed;
        public float CurrentSpeed { get; private set; }


        void Update()
        {
            CheckForInput();
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

            CurrentSpeed = Mathf.Abs(_rigidbody.linearVelocityX);
        }
    }
}
