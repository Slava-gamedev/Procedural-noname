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
            if (!IsBodyMoving())
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
                return GetOppositeLeg();
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
            float direction = _movementController.MovementDirection.x;
            float predictionMultiplier = 1.05f; // 5% extra
            float stepLength = leg.CalculateStepLength(_movementController.CurrentSpeed);
            stepLength = stepLength * predictionMultiplier;

            Vector2 predictedThighPivot = leg.ThighPivotPosition
                             + _movementController.MovementDirection
                             * _movementController.CurrentSpeed * leg.CurrentStepDuration;

            float targetX = predictedThighPivot.x + direction * stepLength;
            float targetY = leg.CurrentFootPosition.y;
            Vector2 target = new Vector2(targetX, targetY);

            float distance = Vector2.Distance(predictedThighPivot, target);
            float legLength = leg.FullLegLenght; 

            if (distance > legLength)
            {
                float height = Mathf.Abs(predictedThighPivot.y - targetY);
                float xMax = Mathf.Sqrt(legLength * legLength - height * height);
                targetX = predictedThighPivot.x + direction * xMax;
                target = new Vector2(targetX, targetY);
            }

            return target;
        }

        private void SetupLegs()
        {
            float speedFactor = _movementController.CurrentSpeed / _movementController.MaxSpeed;
            speedFactor = Mathf.Clamp01(speedFactor);

            _rightLeg.SetStepParameters(speedFactor);
            _leftLeg.SetStepParameters(speedFactor);
        }

        private LegIKController GetOppositeLeg()
        {
            return _lastSteppedLeg == _rightLeg ? _leftLeg : _rightLeg;
        }
    }
}
