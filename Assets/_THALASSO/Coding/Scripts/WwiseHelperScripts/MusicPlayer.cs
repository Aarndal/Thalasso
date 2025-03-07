using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicPlayer : MonoBehaviour
{
#if WWISE_2024_OR_LATER
    [SerializeField]
    private AK.Wwise.Event _mainMenuMusic = default;
    [SerializeField]
    private int[] _scenesToPlayMainMenuMusic;

    [Space(5)]

    [SerializeField]
    private AK.Wwise.Event _creditsMusic = default;
    [SerializeField]
    private int[] _scenesToPlayCreditsMusic;

    private AkGameObj _akGameObject = default;
    private int _activeScene = -1;
    private AK.Wwise.Event _activeAKEvent = null;
    private readonly Dictionary<int, AK.Wwise.Event> _sceneMusic = new();

    private static MusicPlayer _instance;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;

        DontDestroyOnLoad(this.gameObject);

        foreach (var sceneBuildIndex in _scenesToPlayMainMenuMusic)
        {
            _sceneMusic.TryAdd(sceneBuildIndex, _mainMenuMusic);
        }

        foreach (var sceneBuildIndex in _scenesToPlayCreditsMusic)
        {
            _sceneMusic.TryAdd(sceneBuildIndex, _creditsMusic);
        }

        _akGameObject = _akGameObject != null ? _akGameObject : GetComponentInParent<AkGameObj>();

        if (_akGameObject == null)
            _akGameObject = gameObject.AddComponent<AkGameObj>();

        SwitchSceneMusic(SceneManager.GetActiveScene());

        SceneManager.sceneLoaded += OnSceneLoaded;
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

        if (loadedScene.buildIndex == _activeScene)
            return;

        _activeScene = loadedScene.buildIndex;

        SwitchSceneMusic(loadedScene);
    }

    private void SwitchSceneMusic(Scene newScene)
    {
        if (_sceneMusic.TryGetValue(newScene.buildIndex, out AK.Wwise.Event akEvent))
        {
            if (_activeAKEvent != akEvent)
            {
                _activeAKEvent?.Stop(_akGameObject.gameObject);
                akEvent.Post(_akGameObject.gameObject);
                _activeAKEvent = akEvent;
            }
            return;
        }

        _activeAKEvent?.Stop(_akGameObject.gameObject);
    }
#endif
}
