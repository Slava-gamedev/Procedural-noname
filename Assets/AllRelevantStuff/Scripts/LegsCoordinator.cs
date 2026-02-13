using ExtensionMethods;
using UnityEngine;

namespace CharacterMovement
{
    public class LegsCoordinator : MonoBehaviour
    {
        [SerializeField] private CharacterMovementController _movementController;
        [SerializeField] private LegIKController _rightLeg;
        [SerializeField] private LegIKController _leftLeg;
        [SerializeField] private float _stepLength;

        private Vector2 _newFootTargetPosition;
        private LegIKController _footMovingLeg;
        private System.Random _random;

        private void Start()
        {
            _random = new System.Random();
        }

        void Update()
        {
            if(!IsBodyMoving())
                return;
            if (IsLegsAlreadyMoving())
                return;

            if(_footMovingLeg == null)
                ChooseInitialFootMovingLeg();

            if(!IsFootTooFar())
                return;

            CalculateNewFootPosition();
            MoveLeg();
            SwitchFootMovingLeg();
        }

        private bool IsFootTooFar()
        {
            float footDistanceFromBody = Vector2.Distance(transform.position, _footMovingLeg.CurrentFootPosition);
            return footDistanceFromBody > _stepLength;
        }

        private void ChooseInitialFootMovingLeg()
        {
            _footMovingLeg = _random.TossACoin() ? _rightLeg : _leftLeg;
        }

        private void SwitchFootMovingLeg()
        {
            _footMovingLeg = _footMovingLeg == _leftLeg ? _rightLeg : _leftLeg;
        }

        private bool IsLegsAlreadyMoving()
        {
            return _rightLeg.IsMoving || _leftLeg.IsMoving;
        }

        private bool IsBodyMoving()
        {
            return _movementController.CurrentSpeed > 0f;
        }

        private void CalculateNewFootPosition()
        {
            _newFootTargetPosition = _footMovingLeg.CurrentFootPosition;
            Vector2 footPositionOffset = CalculateFootOffset();
            _newFootTargetPosition += footPositionOffset;
        }

        private Vector2 CalculateFootOffset()
        {
            Vector2 footPositionOffset = _movementController.MovementDirection * _stepLength;
            return footPositionOffset;
        }

        private void MoveLeg()
        {
            _footMovingLeg.StartStep(_newFootTargetPosition);
        }
    }
}
