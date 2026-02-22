using ExtensionMethods;
using UnityEngine;

namespace CharacterMovement
{
    public class LegsCoordinator : MonoBehaviour
    {
        private const float kSpeedThreshold = 0.1f;
        private const float kStanceEnd = 0.5f;

        [SerializeField] private CharacterMovementController _movementController;
        [SerializeField] private LegIKController _rightLeg;
        [SerializeField] private LegIKController _leftLeg;

        private LegIKController _lastSteppedLeg;
        private float _previousCycle;
        private System.Random _random = new System.Random();

        void Update()
        {
            if (!IsMoving())
            {
                _previousCycle = _movementController.LocomotionCycle;
                return;
            }

            HandleLegCycle();

            _previousCycle = _movementController.LocomotionCycle;
        }

        private void HandleLegCycle()
        {
            float currentCycle = _movementController.LocomotionCycle;

            float currentRightLegCycle = currentCycle;
            float currentLeftLegCycle = (currentCycle + kStanceEnd) % 1f;

            float previousRightLegCycle = _previousCycle;
            float previousLeftLegCycle = (_previousCycle + kStanceEnd) % 1f;


            TryStepLeg(previousRightLegCycle, currentRightLegCycle, _rightLeg);

            TryStepLeg(previousLeftLegCycle, currentLeftLegCycle, _leftLeg);
        }

        private void TryStepLeg(float previousCycle, float currentCycle, LegIKController leg)
        {
            bool hasEnteredSwing = HasEnteredSwing(previousCycle, currentCycle);
            bool needReset = !leg.IsMoving || IsFootTargetTooFar(leg);

            if (hasEnteredSwing && needReset)
            {
                float stepDuration = CalculateStepDuration();
                Vector2 stepTarget = CalculateStepTarget(leg, stepDuration);
                leg.StartStep(stepTarget, stepDuration);
            }
        }

        private bool HasEnteredSwing(float previousCycle, float currentCycle)
        {
            if(previousCycle <= kStanceEnd && currentCycle >= kStanceEnd)
            {
                return true;
            }

            if (previousCycle > currentCycle)
            {
                return previousCycle <= kStanceEnd || currentCycle >= kStanceEnd;
            }

            return false;
        }


        private Vector2 CalculateStepTarget(LegIKController leg, float stepDuration)
        {
            float direction = Mathf.Sign(_movementController.CurrentSignedSpeed);
            float stepLength = _movementController.CurrentAbsoluteSpeed * stepDuration;

            Vector2 pelvisPos = leg.ThighPivotPosition;
            Vector2 target = pelvisPos + Vector2.right * direction * stepLength;
            target.y = -4.5f;

            return target;
        }

        private bool IsFootTargetTooFar(LegIKController leg)
        {
            float legLength = leg.FullLegLenght;
            Vector2 thighPivot = leg.ThighPivotPosition;
            Vector2 targetStepPosition = leg.TargetFootPosition;

            float distance = Vector2.Distance(thighPivot, targetStepPosition);

            return distance >= legLength;
        }

        private float CalculateStepDuration()
        {
            float cycleSpeed = _movementController.CurrentAbsoluteSpeed
                      * _movementController.CycleFrequency;

            float swingPhaseLength = 1 - kStanceEnd;
            return swingPhaseLength / cycleSpeed;
        }

        private bool IsMoving()
        {
            return _movementController.CurrentAbsoluteSpeed > kSpeedThreshold;
        }


        private void RepositonLegsOnStop()
        {

        }


        private LegIKController GetAppropriateLeg()
        {
            if (_lastSteppedLeg == null)
            {
                bool coinToss = _random.TossACoin();
                return coinToss ? _rightLeg : _leftLeg;
            }

            return _lastSteppedLeg == _rightLeg ? _leftLeg : _rightLeg;
        }
    }
}
