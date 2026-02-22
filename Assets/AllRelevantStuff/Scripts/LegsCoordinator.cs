using UnityEngine;

namespace CharacterMovement
{
    public class LegsCoordinator : MonoBehaviour
    {
        private const float kSpeedThreshold = 0.05f;
        private const float kStanceEnd = 0.5f;

        [SerializeField] private CharacterMovementController _movementController;
        [SerializeField] private LegIKController _rightLeg;
        [SerializeField] private LegIKController _leftLeg;

        private LegIKController _lastSteppedLeg;
        private float _previousCycle;

        void Update()
        {
            if(!IsMoving())
            {
                _previousCycle = _movementController.LocomotionCycle;
                return;
            }

            HandleLegCycle();

            _previousCycle = _movementController.LocomotionCycle;
        }

        private void HandleLegCycle() // TODO
        {
            float currentCycle = _movementController.LocomotionCycle;

            float currentRightLegCycle = currentCycle;
            float currentLeftLegCycle = (currentCycle + 0.5f) % 1f;

            float previousRightLegCycle = _previousCycle;
            float previousLeftLegCycle = (_previousCycle + 0.5f) % 1f;


            TryStepLeg(previousRightLegCycle, currentRightLegCycle, _rightLeg);

            TryStepLeg(previousLeftLegCycle, currentLeftLegCycle, _leftLeg);
        }

        private void TryStepLeg(float previousCycle, float currentCycle, LegIKController leg)
        {
            if (HasEnteredSwing(previousCycle, currentCycle) && !leg.IsMoving)
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
            target.y = leg.CurrentFootPosition.y;

            return target;
        }

        private float CalculateStepDuration()
        {
            float cycleSpeed = _movementController.CurrentAbsoluteSpeed
                      * _movementController.CycleFrequency;

            if (cycleSpeed <= 0.0001f)
                return 0.2f;

            float swingPhaseLength = 0.5f;
            return swingPhaseLength / cycleSpeed;
        }

        private bool IsMoving()
        {
            return _movementController.CurrentAbsoluteSpeed > kSpeedThreshold;
        }
    }
}
