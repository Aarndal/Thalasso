using ProjectTools;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WwiseMusicPlayer : SingletonComponent<MonoBehaviour>
{
#if WWISE_2024_OR_LATER
    [SerializeField]
    private SerializableDictionary<int, SO_WwiseEvent> _sceneMusic = new();

    private AkGameObj _akGameObject = default;
    private SO_WwiseEvent _activeAKEvent = default;
#endif

    protected override void Awake()
    {
        base.Awake();

#if WWISE_2024_OR_LATER
        _akGameObject = _akGameObject != null ? _akGameObject : GetComponentInParent<AkGameObj>();

        if (_akGameObject == null)
            _akGameObject = gameObject.AddComponent<AkGameObj>();
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

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    private void OnSceneLoaded(Scene loadedScene, LoadSceneMode sceneMode)
    {
        if (sceneMode == LoadSceneMode.Additive)
            return;

#if WWISE_2024_OR_LATER
        if (_sceneMusic.Count == 0)
            return;
#endif
        SwitchSceneMusic(loadedScene);
    }

    private void SwitchSceneMusic(Scene newScene)
    {
#if WWISE_2024_OR_LATER
        if (!_sceneMusic.TryGetValue(newScene.buildIndex, out SO_WwiseEvent akEvent))
        {
            if (_activeAKEvent != null)
            {
                _activeAKEvent.Stop(_akGameObject);
                _activeAKEvent = null;
            }
            return;
        }

        if (_activeAKEvent != akEvent)
        {
            if (_activeAKEvent != null)
                _activeAKEvent.Stop(_akGameObject);

            akEvent.Play(_akGameObject);
            _activeAKEvent = akEvent;
        }
        return;
#endif
    }
}
