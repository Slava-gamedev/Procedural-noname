using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CharacterMovement
{
    public class JumpController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CharacterMovementController _movementController;
        [SerializeField] private BodyController _bodyController;
        [SerializeField] private LegsCoordinator _legsCoordinator;
        [SerializeField] private TerrainAnalyzer _terrainAnalyzer;
        [SerializeField] private Rigidbody2D _rigidbody;

        [Header("Values")]
        [SerializeField] private float _jumpForce = 10f;
        [SerializeField] private float _squatOffset = 0.3f;
        [SerializeField] private float _squatTime = 0.2f;
        [SerializeField] private bool _jumpInProgress = false;


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && !_jumpInProgress)
            {
                StartJump().Forget();
            }
        }

        private async UniTask StartJump()
        {
            _jumpInProgress = true;
            Squat();

            int squatTimeInMiliseconds = (int)(1000 * _squatTime);
            await UniTask.Delay(squatTimeInMiliseconds);

            Jump();

            await UniTask.WaitWhile(IsGrounded);
            _legsCoordinator.SetAirMode(_jumpInProgress);

            await WaitUntilGrounded();

            HandleLanding();
        }

        private void Squat()
        {
            _bodyController.SetTemporaryHeightOffset(-_squatOffset);
        }


        private void Jump()
        {
            _bodyController.SetTemporaryHeightOffset(0);

            ApplyForce();
        }

        private void ApplyForce()
        {
            Vector2 forceDirection = Vector2.up;

            Vector2 forceVector = forceDirection * _jumpForce;
            _rigidbody.AddForce(forceVector, ForceMode2D.Impulse);
        }

        private async UniTask WaitUntilGrounded()
        {
            while (_jumpInProgress)
            {
                float velocityY = _rigidbody.linearVelocityY;
                
                if(velocityY < 0f)
                {
                    JumpReport jumpReport = _terrainAnalyzer.CheckJump(transform.position, _bodyController.StandingHeight,
                        _bodyController.BodyRadius,velocityY);

                    if (jumpReport.ShouldPrepareForLanding)
                    {
                        PrepareForLanding();
                    }
                    else if (jumpReport.IsGrounded)
                    {
                        break;
                    }

                }
                
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
            }
        }

        private void PrepareForLanding()
        {

        }

        private void HandleLanding()
        {
            _jumpInProgress = false;
            _legsCoordinator.SetAirMode(_jumpInProgress);
        }

        private bool IsGrounded()
        {
            float velocityY = _rigidbody.linearVelocityY;

            JumpReport jumpReport = _terrainAnalyzer.CheckJump(transform.position, _bodyController.StandingHeight,
                        _bodyController.BodyRadius, velocityY);

            return jumpReport.IsGrounded;
        }
    }
}
