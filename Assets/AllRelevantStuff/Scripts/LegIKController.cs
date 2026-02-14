using UnityEngine;

namespace CharacterMovement
{
    public class LegIKController : MonoBehaviour
    {
        [SerializeField] private Transform _footTarget;
        [SerializeField] private Transform _thighPivot;
        [SerializeField] private Transform _shinPivot;

        [SerializeField] private float _stepDuration = 0.3f;
        [SerializeField] private float _stepHeight = 0.3f;

        private Vector2 _startPosition;
        private Vector2 _targetPosition;
        private float _elapsedTime;

        private float _thighLength;
        private float _shinLength;

        public bool IsMoving { get; private set; }
        public Vector2 CurrentFootPosition => _footTarget.position;

        private void Awake()
        {
            _thighLength = Vector2.Distance(_thighPivot.position, _shinPivot.position);
            _shinLength = Vector2.Distance(_shinPivot.position, _footTarget.position);
        }


        public void StartStep(Vector2 targetPositon)
        {
            _startPosition = CurrentFootPosition;
            _targetPosition = targetPositon;
            _elapsedTime = 0f;
            IsMoving = true;
        }

        private void Update()
        {
            if (IsMoving)
                StepUpdate();
        }

        private void StepUpdate()
        {
            UpdateFootPosition();
            UpdateInverseKinematics();
        }

        private void UpdateFootPosition()
        {
            _elapsedTime += Time.deltaTime;
            float timeParameter = _elapsedTime / _stepDuration;
            Vector2 horizontalPosition = Vector2.Lerp(_startPosition, _targetPosition, timeParameter);
            float heightOffset = Mathf.Sin(timeParameter * Mathf.PI) * _stepHeight;

            _footTarget.position = horizontalPosition + Vector2.up * heightOffset;

            if (timeParameter >= 1)
            {
                _footTarget.position = _targetPosition;
                IsMoving = false;
            }
        }

        private void UpdateInverseKinematics()
        {
            Vector2 vectorToTarget = _footTarget.position - _thighPivot.position;
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
            float thighAngle = angleToTarget - angleOffset;

            Debug.Log($"{_footTarget.gameObject},  thighAngle: {thighAngle},  shinAngle: {180 - kneeAngle}");

            _thighPivot.localRotation = Quaternion.Euler(0, 0, thighAngle);
            _shinPivot.localRotation = Quaternion.Euler(0, 0, 180 - kneeAngle);
        }
    }
}
