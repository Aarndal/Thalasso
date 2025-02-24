using System.Collections;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class StartCutscene : MonoBehaviour
{
    [SerializeField] private SO_GameInputReader _input;
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;
    private GameObject cutsceneCanvas;

    private CinemachineBrain cinemachineBrain;

    private void OnEnable()
    {
        if (cutsceneCanvas == null)
            cutsceneCanvas = GameObject.Find("CutsceneCanvas");

        if (fadeImage == null && cutsceneCanvas != null)
            fadeImage = cutsceneCanvas.GetComponentInChildren<Image>();

        _input.SkipIsTriggered += OnSkipCutScene;
    }

    private void OnDisable()
    {
        _input.SkipIsTriggered -= OnSkipCutScene;
    }

    private void Start()
    {
        cinemachineBrain = Object.FindFirstObjectByType<CinemachineBrain>();
        _input.SwitchCurrentActionMapTo("Cutscene");
        cinemachineBrain.enabled = false;

        fadeImage.gameObject.SetActive(false);
        fadeImage.color = new Color(0, 0, 0, 1);
    }
    private void OnSkipCutScene()
    {
        StartCoroutine(PerformCutsceneSkipWithFade());
    }
    private IEnumerator PerformCutsceneSkipWithFade()
    {
        yield return FadeOut();

        ProcessCutsceneSkip();
        OnCutsceneEnd();

        yield return FadeIn();
    }

    private void ProcessCutsceneSkip()
    {
        Animator eyeAnimator = cutsceneCanvas.GetComponentInChildren<Animator>();

        if (eyeAnimator == null)
            return;

        AnimatorStateInfo stateInfo = eyeAnimator.GetCurrentAnimatorStateInfo(0);
        eyeAnimator.Play(stateInfo.fullPathHash, -1, 1f);
        eyeAnimator.Update(0);
        eyeAnimator.enabled = false;
    }

    public void OnCutsceneEnd()
    {
        PCAnimation pCAnimation = FindAnyObjectByType<PCAnimation>();
        pCAnimation._inCutscene = false;
        pCAnimation.SetAnimationState(0, "Idle", 0.02f);

        cinemachineBrain.enabled = true;
        cinemachineBrain.gameObject.GetComponent<Animator>().enabled = false;
        _input.SwitchCurrentActionMapTo("Player");

        GameObject skipInfoText = cutsceneCanvas.GetComponentInChildren<TextMeshProUGUI>().gameObject;
        skipInfoText.SetActive(false);

        this.enabled = false;
    }


    #region SmoothAnimation
    private IEnumerator FadeOut()
    {
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            float t = 0;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                fadeImage.color = new Color(0, 0, 0, t / fadeDuration);
                yield return null;
            }
            fadeImage.color = new Color(0, 0, 0, 1);
        }
    }

    private IEnumerator FadeIn()
    {
        if (fadeImage != null)
        {
            float t = fadeDuration;
            while (t > 0)
            {
                t -= Time.deltaTime;
                fadeImage.color = new Color(0, 0, 0, t / fadeDuration);
                yield return null;
            }
            fadeImage.color = new Color(0, 0, 0, 0);
            fadeImage.gameObject.SetActive(false);
        }
    }
    #endregion
}
