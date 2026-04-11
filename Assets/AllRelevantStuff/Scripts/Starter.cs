using UnityEngine;
using Zenject;

public class Starter : MonoBehaviour
{

    [Inject]
    private void Construct()
    {
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        Application.targetFrameRate = 60;

    }
}
