using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;

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
       

        private async UniTask StartJump()
        {
            Squat();

            int squatTimeInMiliseconds = (int)(1000 * _squatTime);
            await UniTask.Delay(squatTimeInMiliseconds);

            Jump();
        }

        private void Squat()
        {
            // do squat via BodyController
        }


        private void Jump()
        {
            // Notify relevant scripts about jump
            ApplyForce();
        }

        private void ApplyForce()
        {
            Vector2 forceDirection = Vector2.up;

            Vector2 forceVector = forceDirection * _jumpForce;
            _rigidbody.AddForce(forceVector, ForceMode2D.Impulse);
        }

        private void HandleLanding()
        {
            // Notify relevant scripts about landing
        }
    }
}
