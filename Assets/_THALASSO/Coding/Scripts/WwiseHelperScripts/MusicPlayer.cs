using System.Collections.Generic;
using System.Threading.Tasks;
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

    private AK.Wwise.Event _activeAKEvent = null;
    private readonly Dictionary<int, AK.Wwise.Event> _sceneMusic = new();

    private void Awake()
    {
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
        //SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        //SceneManager.activeSceneChanged -= OnActiveSceneChanged;
    }

    //private void OnActiveSceneChanged(Scene currentScene, Scene nextScene)
    //{
    //    if (_sceneMusic.Count == 0)
    //        return;

    //    SwitchSceneMusic(nextScene);
    //}

    private async void OnSceneLoaded(Scene loadedScene, LoadSceneMode sceneMode)
    {
        if (sceneMode == LoadSceneMode.Additive)
            return;

        if (_sceneMusic.Count == 0)
            return;

        await SwitchSceneMusic(loadedScene);

    }

    private async Task SwitchSceneMusic(Scene newScene)
    {
        while (!newScene.isLoaded)
        {
            await Task.Yield();
        }
        Debug.LogError("Loaded Scene: " + newScene.name);

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
