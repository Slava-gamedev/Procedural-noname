using Cysharp.Threading.Tasks;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

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
        private bool _isPreparingForLanding;

        private void OnEnable()
        {
            InputReader.Actions.InGame.Jump.performed += HandleJumpPress;
        }

        private void OnDisable()
        {
            InputReader.Actions.InGame.Jump.performed -= HandleJumpPress;
        }

        private void HandleJumpPress(CallbackContext callbackContext)
        {
            if (!_jumpInProgress)
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
            _bodyController.SetSpringActive(false);

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
                        _bodyController.BodyRadius, _rigidbody);

                    if (jumpReport.ShouldPrepareForLanding)
                    {
                        PrepareForLanding(jumpReport);
                    }
                    else if (jumpReport.IsGrounded)
                    {
                        break;
                    }
                }
                
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
            }
        }

        private void PrepareForLanding(JumpReport report)
        {
            if (_isPreparingForLanding == false)
            {
                _isPreparingForLanding = true;
                _legsCoordinator.StartLandingPreparation(report.TimeToLanding);
            }
            else
            {
                _legsCoordinator.UpdateLandingPoints();
            }
        }

        private void HandleLanding()
        {
            _jumpInProgress = false;
            _isPreparingForLanding = false;
            _legsCoordinator.SetAirMode(_jumpInProgress);
            _bodyController.SetSpringActive(true);

        }

        private bool IsGrounded()
        {
            JumpReport jumpReport = _terrainAnalyzer.CheckJump(transform.position, _bodyController.StandingHeight,
                        _bodyController.BodyRadius, _rigidbody);

            return jumpReport.IsGrounded;
        }
    }
}
