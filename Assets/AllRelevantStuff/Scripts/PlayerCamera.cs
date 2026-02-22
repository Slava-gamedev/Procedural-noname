using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform _playerCharacter;
    [SerializeField] private float _followSpeed;


    void Update()
    {
        Vector3 targetPosition = _playerCharacter.position;
        targetPosition.z = -10f;

        transform.position = Vector3.Lerp(transform.position, targetPosition, _followSpeed * Time.deltaTime);
    }
}
