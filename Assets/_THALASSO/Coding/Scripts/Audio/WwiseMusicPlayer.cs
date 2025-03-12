using Eflatun.SceneReference;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WwiseMusicPlayer : MonoBehaviour
{
#if WWISE_2024_OR_LATER
    [SerializeField]
    private AK.Wwise.Event _mainMenuMusic = default;
    [SerializeField]
    private SceneReference[] _scenesToPlayMainMenuMusic;

    [Space(5)]

    [SerializeField]
    private AK.Wwise.Event _creditsMusic = default;
    [SerializeField]
    private SceneReference[] _scenesToPlayCreditsMusic;

    private AkGameObj _akGameObject = default;
    private AK.Wwise.Event _activeAKEvent = default;

    private readonly Dictionary<int, AK.Wwise.Event> _sceneMusic = new();

    private static WwiseMusicPlayer _instance = default;

    public static WwiseMusicPlayer Instance => _instance;
    public Dictionary<int, AK.Wwise.Event> SceneMusic => _sceneMusic;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;

        DontDestroyOnLoad(gameObject);

        foreach (var scene in _scenesToPlayMainMenuMusic)
        {
            _sceneMusic.TryAdd(scene.BuildIndex, _mainMenuMusic);
        }

        foreach (var scene in _scenesToPlayCreditsMusic)
        {
            _sceneMusic.TryAdd(scene.BuildIndex, _creditsMusic);
        }

        _akGameObject = _akGameObject != null ? _akGameObject : GetComponentInParent<AkGameObj>();

        if (_akGameObject == null)
            _akGameObject = gameObject.AddComponent<AkGameObj>();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        SwitchSceneMusic(SceneManager.GetActiveScene());
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene loadedScene, LoadSceneMode sceneMode)
    {
        if (sceneMode == LoadSceneMode.Additive)
            return;

        if (_sceneMusic.Count == 0)
            return;

        SwitchSceneMusic(loadedScene);
    }

    private void SwitchSceneMusic(Scene newScene)
    {
        if (_sceneMusic.TryGetValue(newScene.buildIndex, out AK.Wwise.Event akEvent))
        {
            if (_activeAKEvent != akEvent)
            {
                _activeAKEvent?.Stop(_akGameObject.gameObject);
                akEvent?.Post(_akGameObject.gameObject);
                _activeAKEvent = akEvent;
            }
            return;
        }

        _activeAKEvent?.Stop(_akGameObject.gameObject);
        _activeAKEvent = null;
    }
#endif
}
