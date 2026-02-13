using System.Net.NetworkInformation;
using UnityEngine;

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

    public bool IsMoving {  get; private set; }
    public Vector2 CurrentFootPosition => _footTarget.position;



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

    }
}
