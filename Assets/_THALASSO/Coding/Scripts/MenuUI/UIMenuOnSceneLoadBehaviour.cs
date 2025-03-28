using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMenuOnSceneLoadBehaviour : MonoBehaviour
{
    private static bool _isFirstSceneLoad;
    public static bool IsFirstSceneLoad => _isFirstSceneLoad;

    [SerializeField]
    private SO_GameInputReader _input = default;

    [SerializeField]
    private bool _activateOnSceneLoad = false;

    [SerializeField, Tooltip("Set all Canvases that should be loaded when this Canvas is loaded.")]
    private Canvas[] _canvasesToLoad;

    private Canvas _canvas = default;
    private Canvas[] _childCanvases;
    private HashSet<Canvas> _uniqueCanvasesToLoad = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static private void OnBeforeSceneLoad()
    {
        _isFirstSceneLoad = true;
    }

    private void Awake()
    {
        _canvas = _canvas != null ? _canvas : GetComponentInParent<Canvas>();

        _childCanvases = GetComponentsInChildren<Canvas>();

        _uniqueCanvasesToLoad = _canvasesToLoad.ToHashSet();

        _uniqueCanvasesToLoad.Add(_canvas);
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;

    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene loadedScene, LoadSceneMode loadSceneMode)
    {
        if (loadSceneMode == LoadSceneMode.Additive)
            return;

        foreach (var canvas in _childCanvases)
        {
            if (_uniqueCanvasesToLoad.Contains(canvas))
                canvas.enabled = _activateOnSceneLoad;
            else
                canvas.enabled = false;
        }

        //if (_canvas.enabled && !IsFirstSceneLoad) // Added IsFristSceneLoad check to UIMenuSound
        if (_canvas.enabled)
        {
            GlobalEventBus.Raise(GlobalEvents.UI.MenuOpened, _canvas.gameObject.name);
            _input.SwitchCurrentActionMap("UI");
        }

        if (IsFirstSceneLoad)
            _isFirstSceneLoad = false;
    }
}
