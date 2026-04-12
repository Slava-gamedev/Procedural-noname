using UnityEngine;

public interface ILevelDataService
{
    int CurrentLevel { get; }
    bool IsLastLevel { get; }
    void SetLevel(int level);
}

public class LevelDataService : ILevelDataService
{
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
