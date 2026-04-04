using System;
using UnityEngine;

public class LevelTrigger : MonoBehaviour
{
    public Action OnPlayerExitLevel;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnPlayerExitLevel?.Invoke();
    }
}
