using UnityEngine;
using UnityEngine.UI;

public class CreditsAutoScroll : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 0.1f;
    [SerializeField] private SO_GameInputReader _input;
    private ButtonActions buttonActions;
    private ScrollRect scrollRect;

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
        scrollRect.verticalNormalizedPosition -= scrollSpeed * Time.deltaTime;
    }
}
