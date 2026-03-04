using UnityEngine;

namespace CharacterMovement
{
    public class LegsCoordinator : MonoBehaviour
    {
        private const float kSpeedThreshold = 0.1f;
        private const float kStanceCycleEnd = 0.5f;

        [SerializeField] private CharacterMovementController _movementController;
        [SerializeField] private LegIKController _rightLeg;
        [SerializeField] private LegIKController _leftLeg;
        [SerializeField] private GroundDetector _groundDetector;

        private float _previousCycle;

        void Update()
        {
            if (!IsMoving())
            {
                RepositionLegs();
                CacheCurrentCycle();
                return;
            }

            TryStepLeg(_rightLeg, 0f);
            TryStepLeg(_leftLeg, kStanceCycleEnd);

            CacheCurrentCycle();
        }

        private void TryStepLeg(LegIKController leg, float cycleOffset)
        {
            float previousCycle = GetOffsetCycle(_previousCycle, cycleOffset);
            float currentCycle = GetOffsetCycle(_movementController.LocomotionCycle, cycleOffset);

            if (!HasEnteredSwingCycle(previousCycle, currentCycle))
            {
                return;
            }

            if (leg.IsMoving && !IsFootTargetTooFar(leg))
            {
                return;
            }

            float stepDuration = CalculateStepDuration();
            Vector2 stepTarget = CalculateStepTarget(leg, stepDuration);
            leg.UpdateFacingDirection(_movementController.FacingDirection);
            leg.StartStep(stepTarget, stepDuration);
        }

        private bool HasEnteredSwingCycle(float previousCycle, float currentCycle)
        {
            if (previousCycle <= kStanceCycleEnd && currentCycle >= kStanceCycleEnd)
            {
                return true;
            }

            if (previousCycle > currentCycle)
            {
                return previousCycle <= kStanceCycleEnd || currentCycle >= kStanceCycleEnd;
            }

            return false;
        }

        private float GetOffsetCycle(float baseCycle, float offset)
        {
            return (baseCycle + offset) % 1;
        }

        private Vector2 CalculateStepTarget(LegIKController leg, float stepDuration)
        {
            float direction = Mathf.Sign(_movementController.CurrentSignedSpeed);
            float stepLength = _movementController.CurrentAbsoluteSpeed * stepDuration;

            Vector2 pelvisPos = leg.ThighPivotPosition;
            Vector2 target = pelvisPos + Vector2.right * direction * stepLength;
            target.y = _groundDetector.GetGroundHeightAtPosition(target);

            return target;
        }

        private bool IsFootTargetTooFar(LegIKController leg)
        {
            float legLength = leg.FullLegLength;
            Vector2 thighPivot = leg.ThighPivotPosition;
            Vector2 targetStepPosition = leg.TargetFootPosition;

            float distance = Vector2.Distance(thighPivot, targetStepPosition);

            return distance >= legLength;
        }

        private float CalculateStepDuration()
        {
            float currentSpeed = _movementController.CurrentAbsoluteSpeed;
            float cycleFrequency = _movementController.CycleFrequency;
            float cycleSpeed = currentSpeed * cycleFrequency;
            float deceleration = _movementController.Deceleration;

            cycleSpeed = cycleSpeed < 0.01f ? deceleration * cycleFrequency : currentSpeed * cycleFrequency;

            float swingPhaseLength = 1 - kStanceCycleEnd;
            return swingPhaseLength / cycleSpeed;
        }

        private bool IsMoving()
        {
            return _movementController.CurrentAbsoluteSpeed > kSpeedThreshold;
        }

        private void CacheCurrentCycle()
        {
            _previousCycle = _movementController.LocomotionCycle;
        }

        private void TryStepIgnoringCycle(LegIKController leg, LegIKController opposingLeg)
        {
            if (opposingLeg.IsMoving || !IsFootTargetTooFar(leg))
            {
                return;
            }

            float stepDuration = CalculateStepDuration();
            Vector2 stepTarget = CalculateStepTarget(leg, stepDuration);
            leg.UpdateFacingDirection(_movementController.FacingDirection);
            leg.StartStep(stepTarget, stepDuration);
        }

        private void RepositionLegs()
        {
            TryStepIgnoringCycle(_rightLeg, _leftLeg);
            TryStepIgnoringCycle(_leftLeg, _rightLeg);
        }
    }
}
