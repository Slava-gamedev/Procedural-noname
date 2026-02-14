using ExtensionMethods;
using UnityEngine;

namespace CharacterMovement
{
    public class LegsCoordinator : MonoBehaviour
    {
        [SerializeField] private CharacterMovementController _movementController;
        [SerializeField] private LegIKController _rightLeg;
        [SerializeField] private LegIKController _leftLeg;

        private LegIKController _steppingLeg;
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

            _steppingLeg = ChooseLegToStep();

            if (_steppingLeg == null)
                return;

            Vector2 targetPosition = CalculateTargetFootPosition(_steppingLeg);
            _steppingLeg.StartStep(targetPosition);
        }

        private LegIKController ChooseLegToStep()
        {
            float rightDistance = Vector2.Distance(_rightLeg.transform.position, _rightLeg.CurrentFootPosition);
            float leftDistance = Vector2.Distance(_leftLeg.transform.position, _leftLeg.CurrentFootPosition);

            float rightThreshold = _rightLeg.MaxStepLength;
            float leftThreshold = _leftLeg.MaxStepLength;

            bool rightNeedsStep = rightDistance > rightThreshold;
            bool leftNeedsStep = leftDistance > leftThreshold;

            if (rightNeedsStep && leftNeedsStep)
            {
                return _random.TossACoin() ? _rightLeg : _leftLeg;
            }
            else if (rightNeedsStep)
                return _rightLeg;
            else if (leftNeedsStep)
                return _leftLeg;
            else
                return null;
        }

        private bool IsLegsAlreadyMoving()
        {
            return _rightLeg.IsMoving || _leftLeg.IsMoving;
        }

        private bool IsBodyMoving()
        {
            return _movementController.CurrentSpeed > 0f;
        }

        private Vector2 CalculateTargetFootPosition(LegIKController leg)
        {
            Vector2 footPositionOffset = _movementController.MovementDirection * _steppingLeg.MaxStepLength;
            Vector2 target = leg.CurrentFootPosition + footPositionOffset;
            return target;
        }
    }
}
