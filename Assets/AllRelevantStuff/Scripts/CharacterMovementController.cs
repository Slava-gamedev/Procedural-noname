using UnityEngine;

namespace CharacterMovement
{
    public class CharacterMovementController : MonoBehaviour
    {
        [SerializeField] private float _movementSpeed;
        private Vector2 _currentMovementDirection;

        public Vector2 MovementDirection => _currentMovementDirection;
        public float CurrentSpeed { get; private set; }


        void Update()
        {
            CheckForInput();
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
            if(_currentMovementDirection == Vector2.zero)
            {
                CurrentSpeed = 0;
                return;
            }

            CurrentSpeed = _movementSpeed;
            float deltaTime = Time.deltaTime;
            Vector2 movementOffset = (deltaTime * CurrentSpeed) * _currentMovementDirection;
            transform.position += (Vector3)movementOffset;
        }
    }
}
