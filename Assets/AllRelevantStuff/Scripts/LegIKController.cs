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

        [SerializeField] private float _minStepDuration;
        [SerializeField] private float _maxStepDuration;
        [SerializeField] private float _minStrideFactor;
        [SerializeField] private float _maxStrideFactor;
        [SerializeField] private float _minStepTriggerFactor;
        [SerializeField] private float _maxStepTriggerFactor;

        private float _currentStepDuration;
        private float _currentStrideFactor;
        private float _stepTriggerFactor;
        private float _elapsedTime;
        private float _thighLength;
        private float _shinLength;
        private Vector2 _vectorToTarget;
        private Vector2 _startStepPosition;
        private Vector2 _targetStepPosition;
        private float _fullLegLenght => _thighLength + _shinLength;

        public float StepTriggerDistance => StepLength * _stepTriggerFactor;
        public float StepLength => _fullLegLenght * _currentStrideFactor;
        public bool IsMoving { get; private set; }
        public Vector2 CurrentFootPosition => _footTarget.position;
        public Vector2 ThighPivotPosition => _thighPivot.position;


        private void Awake()
        {
            _thighLength = Vector2.Distance(_thighPivot.position, _shinPivot.position);
            _shinLength = Vector2.Distance(_shinPivot.position, _footTarget.position);
            _vectorToTarget = GetCurrentDirectionToTarget();
        }

        public void SetStepParameters(float speedFactor)
        {
            _currentStepDuration = Mathf.Lerp(_maxStepDuration, _minStepDuration, speedFactor);
            _currentStrideFactor = Mathf.Lerp(_minStrideFactor, _maxStrideFactor, speedFactor);
            _stepTriggerFactor = Mathf.Lerp(_maxStepTriggerFactor, _minStepTriggerFactor, speedFactor);
        }

        public void StartStep(Vector2 targetPositon)
        {
            _startStepPosition = CurrentFootPosition;
            _targetStepPosition = targetPositon;
            _elapsedTime = 0f;
            IsMoving = true;
        }

        private void Update()
        {
            if (IsMoving)
            {
                StepUpdate();
                return;
            }
            else if (!IsLegPositionedCorrectly())
            {
                UpdateInverseKinematics();
            }
        }

        private void StepUpdate()
        {
            UpdateFootPosition();
            UpdateInverseKinematics();
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
            Vector2 vectorToTarget = CurrentFootPosition - (Vector2)_thighPivot.position;
            float distance = vectorToTarget.magnitude;
            distance = Mathf.Min(distance, _thighLength + _shinLength);

            float a = _thighLength;
            float b = _shinLength;
            float c = distance;

            float cosKnee = (a * a + b * b - c * c) / (2 * a * b);
            cosKnee = Mathf.Clamp(cosKnee, -1f, 1f); 
            float kneeAngle = Mathf.Acos(cosKnee) * Mathf.Rad2Deg;

            float angleToTarget = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
            float cosThigh = (a * a + c * c - b * b) / (2 * a * c);
            cosThigh = Mathf.Clamp(cosThigh, -1f, 1f);

            float angleOffset = Mathf.Acos(cosThigh) * Mathf.Rad2Deg;
            float thighAngle = angleToTarget - angleOffset + _boneForwardOffset;

            _thighPivot.localRotation = Quaternion.Euler(0, 0, thighAngle);
            _shinPivot.localRotation = Quaternion.Euler(0, 0, 180 - kneeAngle);
            _vectorToTarget = GetCurrentDirectionToTarget();
        }

        private bool IsLegPositionedCorrectly()
        {
            return GetCurrentDirectionToTarget() == _vectorToTarget;
        }

        private Vector2 GetCurrentDirectionToTarget()
        {
            Vector2 vectorToTarget = CurrentFootPosition - (Vector2)_thighPivot.position;
            return vectorToTarget;
        }
    }
}
