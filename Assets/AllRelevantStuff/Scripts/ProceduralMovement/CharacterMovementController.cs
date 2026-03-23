using UnityEngine;

namespace CharacterMovement
{

    public enum FacingDirection
    {
        Left = 0,
        Right = 1,
    }

    public class CharacterMovementController : MonoBehaviour
    {
        private const float kSpeedThreshold = 0.2f;

        [SerializeField] private float _maxMovementSpeed;
        [SerializeField] private float _acceleration;
        [SerializeField] private float _deceleration;
        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private float _cycleFrequency = 1f;
        [SerializeField] private SpriteRenderer _bodySprite;
        private Vector2 _movementDirection;
        private FacingDirection _facingDirection;
        private bool _isMovementBlocked;

        public FacingDirection FacingDirection => _facingDirection;
        public float LocomotionCycle {  get; private set; }
        public float CurrentAbsoluteSpeed { get; private set; }
        public float CurrentSignedSpeed { get; private set; }
        public float CycleFrequency => _cycleFrequency;


        public void SetMovementBlocked(bool isBlocked)
        {
            _isMovementBlocked = isBlocked;
        }

        void Update()
        {
            ReadInput();
            UpdateLocomotionCycle();
        }

        private void FixedUpdate()
        {
            Move();
            StopIfBlocked();
        }

        private void ReadInput()
        {
            float horizontalInput = InputReader.Actions.InGame.Move.ReadValue<float>();

            if (_isMovementBlocked)
            {
                bool tryingToMoveIntoObstacle = (horizontalInput > 0 && FacingDirection == FacingDirection.Right) ||
                                               (horizontalInput < 0 && FacingDirection == FacingDirection.Left);

                if (tryingToMoveIntoObstacle)
                {
                    horizontalInput = 0;
                }
            }

            if(horizontalInput!= 0)
            {
                UpdateSpriteOrientation(horizontalInput);
            }

            _movementDirection = new Vector2(horizontalInput, 0f);
        }

        private void Move()
        {
            float targetSpeed = _movementDirection.x * _maxMovementSpeed;

            float accelRate = (_movementDirection.x != 0)
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
            if (CurrentAbsoluteSpeed < kSpeedThreshold)
            {
                return;
            }

            float deltaPhase = CurrentAbsoluteSpeed * _cycleFrequency * Time.deltaTime;
            LocomotionCycle += deltaPhase;
            LocomotionCycle = Mathf.Repeat(LocomotionCycle, 1);
        }

        private void UpdateSpriteOrientation(float horizontalInput)
        {
            if(horizontalInput == 0)
            {
                return;
            }

            _facingDirection = horizontalInput > 0 ? FacingDirection.Right : FacingDirection.Left;
            _bodySprite.flipX = horizontalInput > 0;
        }

        private void StopIfBlocked()
        {
            if (_isMovementBlocked == false)
            {
                return;
            }

            float currentVelocityX = _rigidbody.linearVelocityX;

            bool movingIntoRightObstacle = (currentVelocityX > 0 && FacingDirection == FacingDirection.Right);
            bool movingIntoLeftObstacle = (currentVelocityX < 0 && FacingDirection == FacingDirection.Left);

            if (movingIntoRightObstacle || movingIntoLeftObstacle)
            {
                _rigidbody.linearVelocity = new Vector2(0f, _rigidbody.linearVelocityY);
            }
        }
    }
}
