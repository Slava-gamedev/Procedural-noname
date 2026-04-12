using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UI;
using UnityEngine;

public interface ISceneUIManager
{
    UniTask ShowSceneAdditive(ISceneMediator mediator);
    UniTask ReleaseScene(ISceneMediator mediator);
}

public class SceneUIManager : ISceneUIManager
{
    private const int kSortingOrderStep = 500;
    private const int kDefaultSortingOrder = 0;

    private List<ISceneMediator> _mediators = new();
    private ISceneMediator _activeSceneMediator;

    private int _sortingOrder = kDefaultSortingOrder;
    private bool _isSceneLoading;

    private Camera Camera
    {
        get
        {
            if (_camera == null)
            {
                _camera = Camera.main;
            }

            return _camera;
        }
    }

    private Camera _camera;


    public async UniTask ShowSceneAdditive(ISceneMediator mediator)
    {
        if (_isSceneLoading)
        {
            await UniTask.WaitWhile(() => _isSceneLoading);
        }

        _isSceneLoading = true;
        if (!_mediators.Contains(mediator))
        {
            _mediators.Add(_activeSceneMediator);
            _sortingOrder += kSortingOrderStep;
            await mediator.Init(Camera, _sortingOrder);
        }

        _activeSceneMediator = mediator;
        mediator.Show();

        _isSceneLoading = false;
    }

    public async UniTask ReleaseScene(ISceneMediator mediator)
    {
        if (_activeSceneMediator.Equals(mediator))
        {
            if (_mediators.Count > 0)
            {
                _activeSceneMediator = _mediators[^1];
                _mediators.RemoveAt(_mediators.Count - 1);
            }
            else
            {
                _activeSceneMediator = null;
            }

            if (_activeSceneMediator != null)
            {
                _activeSceneMediator.Show();
            }

            mediator.Hide();
            UpdateViewOrder();
            mediator.Release();
        }
        else
        {
            if (_mediators.Contains(mediator))
            {
                _mediators.Remove(mediator);

                UpdateViewOrder();
                mediator.Release();
            }
        }
    }

    private void UpdateViewOrder()
    {
        var scenesCount = _mediators.Count;
        var startIndex = 0;
        _sortingOrder = kSortingOrderStep;
        for (int i = startIndex; i < scenesCount; i++)
        {
            var sceneMediator = _mediators[i];
            sceneMediator.ChangeCanvasSorting(_sortingOrder);
            _sortingOrder += kSortingOrderStep;
        }

        if (_activeSceneMediator == null)
        {
            return;
        }

        _activeSceneMediator.ChangeCanvasSorting(_sortingOrder);
        _sortingOrder += kSortingOrderStep;
    }
}
