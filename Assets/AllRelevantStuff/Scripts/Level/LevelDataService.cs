using UnityEngine;

public class LevelDataService
{
    private static LevelDataService _instance;

    private static readonly object _lock = new object();
    public static LevelDataService Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new LevelDataService();
                }
                return _instance;
            }
        }
    }

    private LevelDataService()
    {

    }

    public int CurrentLevel => _currentLevel;
    public bool IsLastLevel => _currentLevel == kAmountOfLevels;


    private const int kAmountOfLevels = 3;
    private int _currentLevel;

    public void SetLevel(int level)
    {
        int clampedLevel = Mathf.Clamp(level, 1, kAmountOfLevels);
        _currentLevel = clampedLevel;
    }

}
