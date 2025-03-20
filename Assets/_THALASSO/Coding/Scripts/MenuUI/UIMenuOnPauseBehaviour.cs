using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIMenuOnPauseBehaviour : MonoBehaviour
{
    [SerializeField, Tooltip("Set all Canvases that should be loaded when this Canvas is loaded.")]
    private Canvas[] _canvasesToLoad;

    private Canvas _canvas = default;
    private Canvas[] _childCanvases;
    private HashSet<Canvas> _uniqueCanvasesToLoad = new();

    private void Awake()
    {
        _canvas = _canvas != null ? _canvas : GetComponentInParent<Canvas>();

        _childCanvases = GetComponentsInChildren<Canvas>();

        _uniqueCanvasesToLoad = _canvasesToLoad.ToHashSet();

        _uniqueCanvasesToLoad.Add(_canvas);
    }

    private void OnEnable()
    {
        GlobalEventBus.Register(GlobalEvents.Game.IsPaused, OnGameIsPaused);
    }

    private void OnDisable()
    {
        GlobalEventBus.Deregister(GlobalEvents.Game.IsPaused, OnGameIsPaused);
    }

    private void OnGameIsPaused(object[] args)
    {
        bool isPauseActive = false;

        foreach (var arg in args)
        {
            if (arg is bool pauseState)
            {
                isPauseActive = pauseState;
                break;
            }
        }

        foreach (var canvas in _childCanvases)
        {
            if (_uniqueCanvasesToLoad.Contains(canvas))
                canvas.enabled = isPauseActive;
            else
                canvas.enabled = false;
        }

        if (_canvas.enabled)
            GlobalEventBus.Raise(GlobalEvents.UI.MenuOpened, _canvas.gameObject.name);

        if (!_canvas.enabled)
            GlobalEventBus.Raise(GlobalEvents.UI.MenuClosed, _canvas.gameObject.name);
    }
}
