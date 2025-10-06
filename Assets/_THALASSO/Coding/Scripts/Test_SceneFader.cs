using UnityEngine;

public class Test_SceneFader : MonoBehaviour
{
    [SerializeField]
    private SO_GameInputReader _input = default;

    private SceneFade _sceneFade = default;

    private void Awake()
    {
        if (!gameObject.TryGetComponent(out _sceneFade))
        {
            _sceneFade = gameObject.GetComponentInChildren<SceneFade>(true);
            if (_sceneFade == null)
                _sceneFade = gameObject.AddComponent<SceneFade>();
        }
    }

    private void OnEnable()
    {
        _input.InteractIsTriggered += OnInteractIsTriggered;
        _sceneFade.FadeIsExecuted += OnFadeIsExecuted;
        _sceneFade.FadeStopped += OnFadeStopped;
        _sceneFade.FadeIsCanceled += OnFadeWasCanceled;
    }

    private void OnInteractIsTriggered(bool obj)
    {
        _sceneFade.CancelFade();
    }

    private void Start()
    {
        _sceneFade.StartFadeCoroutine(SceneFade.FadeMode.FadeIn, 5.0f);
    }

    private void OnDisable()
    {
        _sceneFade.FadeIsCanceled -= OnFadeWasCanceled;
        _sceneFade.FadeStopped -= OnFadeStopped;
        _sceneFade.FadeIsExecuted -= OnFadeIsExecuted;
        _input.InteractIsTriggered -= OnInteractIsTriggered;

    }

    private void OnFadeWasCanceled()
    {
        Debug.LogError("Fade was canceled.");
    }

    private void OnFadeStopped(SceneFade.FadeMode mode)
    {
        Debug.LogError("Fade stopped.");
    }

    private void OnFadeIsExecuted(SceneFade.FadeMode mode)
    {
        Debug.LogError("Fade is executed.");
    }
}
