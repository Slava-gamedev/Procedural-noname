using UnityEngine;

namespace CharacterMovement
{
    public class BodyController : MonoBehaviour
    {
        [SerializeField] private CharacterMovementController _movementController;
        [SerializeField] private Transform _bodyTransform;
        [SerializeField] private float _bodyAmplitude = 0.05f;
        [SerializeField] private float _baseHeight = 0f;

        void Update()
        {
            float cycle = _movementController.LocomotionCycle;

            float verticalOffset = -Mathf.Cos(cycle * Mathf.PI * 2) * _bodyAmplitude;

            Vector3 localPos = _bodyTransform.localPosition;
            localPos.y = _baseHeight + verticalOffset;
            _bodyTransform.localPosition = localPos;
        }
    }
}
