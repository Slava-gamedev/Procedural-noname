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
            ApplyVerticalOffset();
        }

        private void ApplyVerticalOffset()
        {
            float cycle = _movementController.LocomotionCycle;
            float verticalOffset = CalculateVerticalOffset(cycle);

            Vector3 localPosition = _bodyTransform.localPosition;
            localPosition.y = _baseHeight + verticalOffset;

            _bodyTransform.localPosition = localPosition;
        }

        private float CalculateVerticalOffset(float cycle)
        {
            return -Mathf.Cos(cycle * Mathf.PI * 2f) * _bodyAmplitude;
        }
    }
}
