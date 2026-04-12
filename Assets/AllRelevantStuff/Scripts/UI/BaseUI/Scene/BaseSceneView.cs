using UI;
using UnityEngine;

public interface ISceneView : IView
{
    void Init(Camera camera, int sortingOrder);
    void ChangeCanvasSorting(int sortingOrder);
}

public class BaseSceneView : MonoBehaviour, ISceneView
{

    [SerializeField] protected Canvas _canvas;

    public void Init(Camera camera, int sortingOrder)
    {
        _canvas.worldCamera = camera;
        _canvas.sortingOrder = sortingOrder;
    }

    public void Show()
    {
        gameObject.SetActive(true); 
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public virtual void Release()
    {
    }

    public void ChangeCanvasSorting(int sortingOrder)
    {
        _canvas.sortingOrder = sortingOrder;
    }
}
