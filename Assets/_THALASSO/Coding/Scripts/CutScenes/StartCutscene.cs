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
    private GameObject startCutsceneCanvas;

    private CinemachineBrain cinemachineBrain;
    private PCAnimation pCAnimation;

    private void OnEnable()
    {
        if (startCutsceneCanvas == null)
            startCutsceneCanvas = GameObject.Find("StartCutsceneCanvas");

        if (fadeImage == null && startCutsceneCanvas != null)
            fadeImage = startCutsceneCanvas.GetComponentInChildren<Image>();

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
        Debug.Log("StartSkip");
        yield return FadeOut();

        Debug.Log("StartProcessing");
        ProcessCutsceneSkip();
        OnCutsceneEnd();
        Debug.Log("EndProcessing");

        yield return FadeIn();
        Debug.Log("EndSkip");
    }

    private void ProcessCutsceneSkip()
    {
        Debug.Log("StartProcessing");
        Animator eyeAnimator = startCutsceneCanvas.GetComponentInChildren<Animator>();
        GameObject skipInfoText = startCutsceneCanvas.GetComponentInChildren<TextMeshProUGUI>().gameObject;

        if (eyeAnimator == null || skipInfoText == null)
            return;

        AnimatorStateInfo stateInfo = eyeAnimator.GetCurrentAnimatorStateInfo(0);
        eyeAnimator.Play(stateInfo.fullPathHash, -1, 1f);
        eyeAnimator.Update(0);
        eyeAnimator.enabled = false;

        skipInfoText.SetActive(false);

    }

    public void OnCutsceneEnd()
    {
        PCAnimation pCAnimation = FindAnyObjectByType<PCAnimation>();
        pCAnimation._inCutscene = false;
        pCAnimation.SetAnimationState(0, "Idle", 0.02f);

        cinemachineBrain.enabled = true;
        cinemachineBrain.gameObject.GetComponent<Animator>().enabled = false;
        _input.SwitchCurrentActionMapTo("Player");
    }


    #region SmoothAnimation
    private IEnumerator FadeOut()
    {
        Debug.Log("StartFadeOut");
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            float t = 0;
            while (t < fadeDuration)
            {
                Debug.Log("DuringFadeOut");
                t += Time.deltaTime;
                fadeImage.color = new Color(0, 0, 0, t / fadeDuration);
                yield return null;
            }
            fadeImage.color = new Color(0, 0, 0, 1);

        }
        Debug.Log("EndFadeOut");
    }

    private IEnumerator FadeIn()
    {
        Debug.Log("StartFadeIn");
        if (fadeImage != null)
        {
            float t = fadeDuration;
            while (t > 0)
            {
                Debug.Log("DuringFadeIn");
                t -= Time.deltaTime;
                fadeImage.color = new Color(0, 0, 0, t / fadeDuration);
                yield return null;
            }
            fadeImage.color = new Color(0, 0, 0, 0);
            fadeImage.gameObject.SetActive(false);
        }
        Debug.Log("EndFadeIn");
    }
    #endregion
}
