using Eflatun.SceneReference;
using ProjectTools;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WwiseMusicPlayer : SingletonComponent<MonoBehaviour>
{
#if WWISE_2024_OR_LATER
    [SerializeField]
    private SerializableDictionary<string, SO_WwiseEvent> _sceneMusic = new();

    private AkGameObj _akGameObject = default;
    private SO_WwiseEvent _activeAKEvent = default;

    public readonly Dictionary<SceneReference, SO_WwiseEvent> SceneMusic = new();
#endif

    protected override void Awake()
    {
        base.Awake();

#if WWISE_2024_OR_LATER
        _akGameObject = _akGameObject != null ? _akGameObject : GetComponentInParent<AkGameObj>();

        if (_akGameObject == null)
            _akGameObject = gameObject.AddComponent<AkGameObj>();

        foreach (var scene in _sceneMusic)
        {
            if (!SceneMusic.TryAdd(new SceneReference(SceneManager.GetSceneByName(scene.Key).path), scene.Value))
            {
                Debug.LogErrorFormat("{0}'s {1} component failed to add scene to SceneMusic!", gameObject.name, this);
            }
        }
#endif
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        SwitchSceneMusic(SceneManager.GetActiveScene());
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene loadedScene, LoadSceneMode sceneMode)
    {
        if (sceneMode == LoadSceneMode.Additive)
            return;

#if WWISE_2024_OR_LATER
        if (SceneMusic.Count == 0)
            return;
#endif
        SwitchSceneMusic(loadedScene);
    }

    private void SwitchSceneMusic(Scene newScene)
    {
#if WWISE_2024_OR_LATER
        SceneReference scene = SceneMusic.Keys.First((sceneRef) => sceneRef.BuildIndex == newScene.buildIndex);

        if (scene != null)
        {
            SO_WwiseEvent akEvent = SceneMusic[scene];

            if (_activeAKEvent != akEvent)
            {
                _activeAKEvent.Stop(_akGameObject);
                akEvent.Play(_akGameObject);
                _activeAKEvent = akEvent;
            }
            return;
        }

        _activeAKEvent.Stop(_akGameObject);
        _activeAKEvent = null;
#endif
    }
}
