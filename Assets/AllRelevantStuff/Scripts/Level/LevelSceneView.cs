using UnityEngine;

public class LevelSceneView : BaseSceneView
{
    [SerializeField] private LevelTrigger _trigger;

    public LevelTrigger Trigger => _trigger;
}
