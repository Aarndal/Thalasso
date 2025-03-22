using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneFade : MonoBehaviour
{
    [Flags]
    public enum FadeMode
    {
        None = 0,
        FadeIn = 1 << 0,
        FadeOut = 1 << 1,
    }

    [SerializeField]
    private Image _fadeImage = default;
    [SerializeField]
    private FadeMode _fadeMode = FadeMode.None;
    [SerializeField, Range(0.0f, 2.0f)]
    private float _fadeInDuration = 0.5f;
    [SerializeField, Range(0.0f, 2.0f)]
    private float _fadeOutDuration = 0.5f;

    private Canvas _canvas = default;
    private Color _opaqueColor = Color.black;

    private void Awake()
    {
        if (!gameObject.TryGetComponent(out _fadeImage))
        {
            _fadeImage = gameObject.AddComponent<Image>();
            _fadeImage.color = Color.black;
        }

        if (!gameObject.TryGetComponent(out _canvas))
            _canvas = gameObject.GetComponentInParent<Canvas>(true);

        _opaqueColor = _fadeImage.color;
        _fadeImage.color = Color.clear;

        if ((_fadeMode & FadeMode.FadeIn) == 0)
            StartCoroutine(Fade(FadeMode.FadeIn, _fadeInDuration));
        //SceneManager.sceneLoaded += OnSceneLoaded;
        //SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void Start()
    {
        //if ((_fadeMode & FadeMode.FadeIn) == 0)
            _canvas.enabled = false;
    }

    private void OnDestroy()
    {
        //    SceneManager.sceneUnloaded -= OnSceneUnloaded;
        //    SceneManager.sceneLoaded -= OnSceneLoaded;

        StopAllCoroutines();
    }

//private void OnSceneLoaded(Scene loadedScene, LoadSceneMode loadSceneMode)
//{
//    if (_fadeImage == null || (_fadeMode & FadeMode.FadeIn) == 0)
//        return;

//    if (loadSceneMode == LoadSceneMode.Additive)
//        return;

//    StartCoroutine(new WaitUntil(() => Fade(FadeMode.FadeIn, _fadeInDuration)));
//}

//private void OnSceneUnloaded(Scene unloadedScene)
//{
//    if (_fadeImage == null || (_fadeMode & FadeMode.FadeOut) == 0)
//        return;

//    if (unloadedScene != SceneManager.GetActiveScene())
//        return;

//    StartCoroutine(new WaitUntil (() => Fade(FadeMode.FadeOut, _fadeOutDuration)));
//}


private IEnumerator<bool> Fade(FadeMode fadeMode, float fadeDuration)
    {
        _fadeImage.color = fadeMode == FadeMode.FadeIn ? _opaqueColor : Color.clear;

        float startAlpha = _fadeImage.color.a;

        _canvas.enabled = true;

        float counter = 0.0f;

        while (counter < fadeDuration)
        {
            counter += Time.deltaTime;

            _fadeImage.color = new Color(_opaqueColor.r, _opaqueColor.g, _opaqueColor.b, Mathf.Lerp(startAlpha, 1.0f - startAlpha, counter / fadeDuration));

            yield return false;
        }

        _fadeImage.color = fadeMode == FadeMode.FadeOut ? _opaqueColor : Color.clear;

        _canvas.enabled = false;

        yield return true;
    }
}
