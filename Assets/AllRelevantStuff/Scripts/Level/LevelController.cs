using UI;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField] private LevelTrigger _trigger;
    [SerializeField] private LevelCompletedMenuController _menuController;

    void Start()
    {
        _trigger.OnPlayerExitLevel += OnLevelEnd;
    }

    private void OnLevelEnd()
    {
        _menuController.Show();
        _trigger.OnPlayerExitLevel -= OnLevelEnd;
    }
}
