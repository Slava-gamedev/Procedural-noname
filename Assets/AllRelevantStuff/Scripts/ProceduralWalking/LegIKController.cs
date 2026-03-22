using UnityEngine;

namespace CharacterMovement
{
    public class LegIKController : MonoBehaviour
    {
        [SerializeField] private float _boneForwardOffset = 90f;
        [SerializeField] private Transform _footTarget;
        [SerializeField] private Transform _thighPivot;
        [SerializeField] private Transform _shinPivot;

        [SerializeField] private float _stepHeight = 0.3f;
        [SerializeField] private Vector2 _airPoseOffset;
        [SerializeField] private float _tuckTime;

        private float _currentStepDuration;
        private float _elapsedTime;
        private float _thighLength;
        private float _shinLength;
        private Vector2 _cachedDirectionToTarget;
        private Vector2 _startStepPosition;
        private Vector2 _targetStepPosition;
        private float _angleDirectionModifier;
        private bool _isInAirPose = false;
        private Vector2 _startLocalOffset;

        public float FullLegLength => _thighLength + _shinLength;
        public bool IsMoving { get; private set; }
        public Vector2 CurrentFootPosition => _footTarget.position;
        public Vector2 ThighPivotPosition => _thighPivot.position;
        public Vector2 TargetFootPosition => _targetStepPosition;

        private void Awake()
        {
            _thighLength = Vector2.Distance(_thighPivot.position, _shinPivot.position);
            _shinLength = Vector2.Distance(_shinPivot.position, _footTarget.position);
            _cachedDirectionToTarget = GetCurrentDirectionToTarget();
        }

        public void SetInAirPose(bool inAir)
        {
            _isInAirPose = inAir;
            if (inAir)
            {
                IsMoving = false;
                _elapsedTime = 0f;
                _startLocalOffset = (Vector2)_footTarget.position - (Vector2)_thighPivot.position;
            }
        }

        public void UpdateFacingDirection(FacingDirection facingDirection)
        {
            _angleDirectionModifier = facingDirection == FacingDirection.Right ? 1 : -1;
        }

        public void StartStep(Vector2 targetPositon, float stepDuration)
        {
            _currentStepDuration = stepDuration;
            _startStepPosition = CurrentFootPosition;
            _targetStepPosition = targetPositon;

            _elapsedTime = 0f;
            IsMoving = true;
        }

        private void Update()
        {
            if (_isInAirPose)
            {
                AirPoseUpdate();
                UpdateInverseKinematics();
                return;
            }

            if (IsMoving)
            {
                StepUpdate();
            }
            else if (!IsLegAlignedWithTarget())
            {
                UpdateInverseKinematics();
            }
        }

        private void StepUpdate()
        {
            UpdateFootPosition();
            UpdateInverseKinematics();
        }

        private void AirPoseUpdate()
        {
            Vector2 targetOffset = new Vector2(_airPoseOffset.x * _angleDirectionModifier, _airPoseOffset.y);

            _elapsedTime += Time.deltaTime;
            float timeParameter = _elapsedTime / _tuckTime;
            float smoothT = Mathf.SmoothStep(0, 1, timeParameter);
            Vector2 offset = Vector2.Lerp(_startLocalOffset, targetOffset, smoothT);

            _footTarget.position = (Vector2)_thighPivot.position + offset;
        }

        private void UpdateFootPosition()
        {
            _elapsedTime += Time.deltaTime;
            float timeParameter = _elapsedTime / _currentStepDuration;
            Vector2 horizontalPosition = Vector2.Lerp(_startStepPosition, _targetStepPosition, timeParameter);
            float heightOffset = Mathf.Sin(timeParameter * Mathf.PI) * _stepHeight;

            _footTarget.position = horizontalPosition + Vector2.up * heightOffset;

            if (timeParameter >= 1)
            {
                _footTarget.position = _targetStepPosition;
                IsMoving = false;
            }
        }

        private void UpdateInverseKinematics()
        {
            Vector2 toTarget = GetCurrentDirectionToTarget();
            float distance = Mathf.Min(toTarget.magnitude, FullLegLength);

            float thighAngle = CalculateThighAngle(toTarget, distance);
            float shinAngle = CalculateShinAngle(distance);

            _thighPivot.localRotation = Quaternion.Euler(0, 0, thighAngle);
            _shinPivot.localRotation = Quaternion.Euler(0, 0, shinAngle);
            _cachedDirectionToTarget = GetCurrentDirectionToTarget();
        }

        private float CalculateThighAngle(Vector2 toTarget, float distance)
        {
            float a = _thighLength;
            float b = _shinLength;
            float c = distance;

            float angleToTarget =
                Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg;

            float cosThigh =
                (a * a + c * c - b * b) / (2f * a * c);

            cosThigh = Mathf.Clamp(cosThigh, -1f, 1f);

            float offset =
                Mathf.Acos(cosThigh) * Mathf.Rad2Deg;

            float thighAngle = angleToTarget + (offset * _angleDirectionModifier) + _boneForwardOffset;

            return thighAngle;
        }

        private float CalculateShinAngle(float distance)
        {
            float a = _thighLength;
            float b = _shinLength;
            float c = distance;

            float cosKnee = (a * a + b * b - c * c) / (2f * a * b);

            cosKnee = Mathf.Clamp(cosKnee, -1f, 1f);

            float kneeAngle =
                Mathf.Acos(cosKnee) * Mathf.Rad2Deg;

            return (180f - kneeAngle) * (_angleDirectionModifier * -1f);
        }

        private bool IsLegAlignedWithTarget()
        {
            return Vector2.Distance(GetCurrentDirectionToTarget(), _cachedDirectionToTarget) < 0.0001f;
        }

        private Vector2 GetCurrentDirectionToTarget()
        {
            Vector2 vectorToTarget = CurrentFootPosition - (Vector2)_thighPivot.position;
            return vectorToTarget;
        }
    }
}
