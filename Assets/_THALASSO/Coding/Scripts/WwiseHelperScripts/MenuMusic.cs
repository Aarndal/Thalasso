using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuMusic : MonoBehaviour
{
    [SerializeField]
    private AkGameObj _akGameObj;
    [SerializeField]
    private AK.Wwise.Event _menuMusic;
    [SerializeField]
    private uint[] _sceneIndicesToStartMusic;
    [SerializeField]
    private uint[] _sceneIndicesToEndMusic;

    private uint[] playingIds = new uint[50];

    private void Awake()
    {
        if (_akGameObj == null)
        {
            _akGameObj = GetComponentInChildren<AkGameObj>();
        }
    }

    private void Start()
    {
        if (!IsEventPlayingOnGameObject(_menuMusic, _akGameObj.gameObject))
        {
            Scene currentScene = SceneManager.GetActiveScene();

            foreach (var sceneIndex in _sceneIndicesToStartMusic)
            {
                if (currentScene.buildIndex == sceneIndex)
                {
                    _menuMusic.Post(_akGameObj.gameObject);
                    return;
                }
            }
        }
    }

    private void OnEnable()
    {
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
    }

    private void OnActiveSceneChanged(Scene currentScene, Scene nextScene)
    {
        if (!IsEventPlayingOnGameObject(_menuMusic, _akGameObj.gameObject))
        {
            foreach (var sceneIndex in _sceneIndicesToStartMusic)
            {
                if (nextScene.buildIndex == sceneIndex)
                {
                    _menuMusic.Post(_akGameObj.gameObject);
                    return;
                }
            }
        }

        if (IsEventPlayingOnGameObject(_menuMusic, _akGameObj.gameObject))
        {
            foreach (var sceneIndex in _sceneIndicesToEndMusic)
            {
                if (nextScene.buildIndex == sceneIndex)
                {
                    _menuMusic.Stop(_akGameObj.gameObject);
                    return;
                }
            }
        }
    }

    private bool IsEventPlayingOnGameObject(AK.Wwise.Event @event, GameObject gameObject)
    {
        uint count = (uint)playingIds.Length;
        AKRESULT result = AkUnitySoundEngine.GetPlayingIDsFromGameObject(gameObject, ref count, playingIds);

        for (int i = 0; i < count; i++)
        {
            uint playingId = playingIds[i];
            uint eventId = AkUnitySoundEngine.GetEventIDFromPlayingID(playingId);

            if (eventId == @event.Id)
                return true;
        }

        return false;
    }
}
