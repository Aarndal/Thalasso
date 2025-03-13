using UnityEngine;
using UnityEngine.UI;

public class CreditsAutoScroll : MonoBehaviour
{
    [SerializeField] private float scrollDuration = 20f;
    [SerializeField] private AnimationCurve scrollCurve;
    [SerializeField] private float timeToSwitchAtEnd = 1.5f;
    [SerializeField] private SO_GameInputReader _input;
    private ButtonActions buttonActions;
    private ScrollRect scrollRect;
    private float elapsedTime = 0f;

    private void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
        buttonActions = FindAnyObjectByType<ButtonActions>();
        _input.SkipIsTriggered += OnSkipCutScene;
    }

    private void OnDisable()
    {
        _input.SkipIsTriggered -= OnSkipCutScene;
    }

    private void OnSkipCutScene()
    {
        buttonActions.LoadScene(0);
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime < scrollDuration)
        {
            
            float t = elapsedTime / scrollDuration;
            float curveValue = scrollCurve.Evaluate(t);
            scrollRect.verticalNormalizedPosition = Mathf.Lerp(1, 0, curveValue);
        }
        else if( elapsedTime > scrollDuration + 1)
        {
            OnSkipCutScene();
        }
    }
}
