using UnityEngine;

namespace CharacterMovement
{
    public class LegsCoordinator : MonoBehaviour
    {
        [SerializeField] private CharacterMovementController _movementController;
        [SerializeField] private LegIKController _rightLeg;
        [SerializeField] private LegIKController _leftLeg;

        private LegIKController _lastSteppedLeg;

        void Update()
        {
            if(!IsBodyMoving())
                return;
            if (IsLegsAlreadyMoving())
                return;

            SetupLegs();

            LegIKController leg = ChooseLegToStep();

            if (leg == null)
                return;

            Vector2 targetPosition = CalculateTargetFootPosition(leg);
            leg.StartStep(targetPosition);
            _lastSteppedLeg = leg;
        }

        private LegIKController ChooseLegToStep()
        {
            float rightDistance = Mathf.Abs(_rightLeg.ThighPivotPosition.x - _rightLeg.CurrentFootPosition.x);
            float leftDistance = Mathf.Abs(_leftLeg.ThighPivotPosition.x - _leftLeg.CurrentFootPosition.x);

            float rightThreshold = _rightLeg.StepTriggerDistance;
            float leftThreshold = _leftLeg.StepTriggerDistance;

            bool rightNeedsStep = rightDistance > rightThreshold;
            bool leftNeedsStep = leftDistance > leftThreshold;

            if (rightNeedsStep && leftNeedsStep)
            {
                return _lastSteppedLeg == _rightLeg ? _leftLeg : _rightLeg;
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
            float targetX = leg.ThighPivotPosition.x + _movementController.MovementDirection.x * leg.StepLength;
            float targetY = leg.CurrentFootPosition.y;
            Vector2 target = new Vector2(targetX, targetY);
            return target;
        }

        private void SetupLegs()
        {
            float speedFactor = _movementController.CurrentSpeed / _movementController.MaxSpeed;
            speedFactor = Mathf.Clamp01(speedFactor);

            _rightLeg.SetStepParameters(speedFactor);
            _leftLeg.SetStepParameters(speedFactor);
        }
    }
}
