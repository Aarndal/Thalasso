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
                canvas.enabled = _activateOnSceneLoad;
            else
                canvas.enabled = false;
        }
    }

    private void Start()
    {
        if (_canvas.enabled)
            GlobalEventBus.Raise(GlobalEvents.UI.MenuOpened, _canvas.gameObject.name);
    }
}
