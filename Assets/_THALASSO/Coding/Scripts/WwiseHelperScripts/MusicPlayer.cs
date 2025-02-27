using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField]
    private AK.Wwise.Event _mainMenuMusic = default;
    [SerializeField]
    private int[] _scenesToPlayMainMenuMusic;

    [Space(5)]

    [SerializeField]
    private AK.Wwise.Event _creditsMusic = default;
    [SerializeField]
    private int[] _scenesToPlayCreditsMusic;

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

        foreach (var sceneBuildIndex in _scenesToPlayMainMenuMusic)
        {
            _sceneMusic.TryAdd(sceneBuildIndex, _mainMenuMusic);
        }

        foreach (var sceneBuildIndex in _scenesToPlayCreditsMusic)
        {
            _sceneMusic.TryAdd(sceneBuildIndex, _creditsMusic);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
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
            if(_activeAKEvent != akEvent)
            {
                _activeAKEvent?.Stop(gameObject);
                akEvent.Post(gameObject);
                _activeAKEvent = akEvent;
            }
            return;
        }

        _activeAKEvent?.Stop(gameObject);
    }
}
