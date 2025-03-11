using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIMenuOnSceneLoadBehaviour : MonoBehaviour
{
    [SerializeField]
    private bool _activateOnSceneLoad = false;

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
        foreach (var canvas in _childCanvases)
        {
            if (_uniqueCanvasesToLoad.Contains(canvas))
                canvas.enabled = true;
            else
                canvas.enabled = false;
        }

        _canvas.enabled = _activateOnSceneLoad;

        if (_canvas.enabled)
            GlobalEventBus.Raise(GlobalEvents.UI.MenuOpened, _canvas.gameObject.name);
    }

    private void OnDisable()
    {
        if (_canvas.enabled)
            GlobalEventBus.Raise(GlobalEvents.UI.MenuClosed, _canvas.gameObject.name);
    }
}
