using Eflatun.SceneReference;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreditsAutoScroll : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SO_GameInputReader _input;
    [SerializeField] private SceneReference _sceneToLoad;

    [Space(5)]

    [SerializeField] private AnimationCurve scrollCurve;
    [SerializeField] private float scrollDuration = 20f;
    [SerializeField] private float timeToSwitchAtEnd = 1.5f;

    private ScrollRect scrollRect;
    private float elapsedTime = 0f;

    private void Awake()
    {
        _input.SwitchCurrentActionMap("Cutscene");
        scrollRect = GetComponentInChildren<ScrollRect>();
    }

    private void OnEnable()
    {
        _input.SkipIsPerformed += OnSkipCutScene;
    }

    private void OnDisable()
    {
        _input.SkipIsPerformed -= OnSkipCutScene;
    }

    private void OnSkipCutScene()
    {
        SceneManager.LoadSceneAsync(_sceneToLoad.BuildIndex);
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime <= scrollDuration)
        {
            float t = elapsedTime / scrollDuration;
            float curveValue = scrollCurve.Evaluate(t);
            scrollRect.verticalNormalizedPosition = Mathf.Lerp(1, 0, curveValue);
        }

        if (elapsedTime > scrollDuration + timeToSwitchAtEnd)
        {
            OnSkipCutScene();
        }
    }
}
