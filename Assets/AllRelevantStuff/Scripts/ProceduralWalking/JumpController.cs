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

        [SerializeField] private float _jumpForce = 10f;
        [SerializeField] private float _squatOffset = 0.3f;
        [SerializeField] private float _squatTime = 0.2f;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
