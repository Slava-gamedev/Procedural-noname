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
        [SerializeField] private TerrainAnalyzer _terrainAnalyzer;

        [SerializeField] private float _minStepDuration;
        [SerializeField] private float _maxStepDuration;
        private float _previousCycle;
        private bool _airMode;

        public void StartLandingPreparation(Vector2 landingPoint, float timeBeforeLanding)
        {
            if (_airMode)
            {
                Debug.Log($"landingPoint: {landingPoint}, timeBeforeLanding: {timeBeforeLanding}");

                var landingTargets = CalculateLandingTargets(landingPoint);

                _leftLeg.StartPreparingForLanding(landingTargets.leftTarget, timeBeforeLanding);
                _rightLeg.StartPreparingForLanding(landingTargets.rightTarget, timeBeforeLanding);
            }
        }

        public void UpdateLandingPoints(Vector2 landingPoint)
        {
            var landingTargets = CalculateLandingTargets(landingPoint);

            _leftLeg.UpdateLandingTarget(landingTargets.leftTarget);
            _rightLeg.UpdateLandingTarget(landingTargets.rightTarget);
        }


        public void SetAirMode(bool airMode)
        {
            Debug.Log($"airMode: {airMode}");

            _airMode = airMode;
            _rightLeg.SetInAir(_airMode);
            _leftLeg.SetInAir(_airMode);
            if (!_airMode)
            {
                RepositionLegs();
            }
        }

        private void Update()
        {
            if (_airMode)
            {
                return;
            }

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
            int direction = (_movementController.CurrentSignedSpeed >= 0f) ? 1 : (-1);
            float stepLength = _movementController.CurrentAbsoluteSpeed * stepDuration;

            Vector2 pelvisPos = leg.ThighPivotPosition;
            Vector2 target = _terrainAnalyzer.GetStepTarget(pelvisPos, stepLength, direction);
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
            float minEffectiveSpeed = 0.5f;
            float currentSpeed = _movementController.CurrentAbsoluteSpeed;
            currentSpeed = Mathf.Max(currentSpeed, minEffectiveSpeed);

            float cycleFrequency = _movementController.CycleFrequency;
            float cycleSpeed = currentSpeed * cycleFrequency;

            cycleSpeed = currentSpeed * cycleFrequency;

            float swingPhaseLength = 1 - kStanceCycleEnd;
            float stepDuration = swingPhaseLength / cycleSpeed;

            stepDuration = Mathf.Clamp(stepDuration, _minStepDuration, _maxStepDuration);

            return stepDuration;
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

        private (Vector2 leftTarget, Vector2 rightTarget) CalculateLandingTargets(Vector2 landingPoint)
        {
            Vector2 origin = transform.position;

            float leftLegOffset = (_leftLeg.ThighPivotPosition - origin).x;
            Vector2 leftTarget = landingPoint + new Vector2(leftLegOffset, 0);
            Vector2 rightTarget = landingPoint + new Vector2(-leftLegOffset, 0);

            return (leftTarget, rightTarget);
        }
    }
}
