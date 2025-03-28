using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SceneFade : MonoBehaviour
{
    public enum FadeMode
    {
        None = 0,
        FadeIn = 1,
        FadeOut = 2,
    }

    [SerializeField]
    private Image _fadeImage = default;
    [SerializeField, Range(0.01f, 5.0f)]
    private float _defaultFadeDuration = 1.0f;

    private bool _isFading = false;
    private Canvas _canvas = default;
    private Color _opaqueColor = Color.black;
    private FadeMode _currentFadeMode = FadeMode.None;

    private CancellationTokenSource _activeFadeCTS = default;
    private Coroutine _activeFadeCoroutine = default;

    public bool IsFading
    {
        get => _isFading;
        private set
        {
            if (value == _isFading)
                return;

            _isFading = value;

            if (_isFading)
                FadeIsExecuted?.Invoke(_currentFadeMode);
            else
                FadeStopped?.Invoke(_currentFadeMode);
        }
    }

    public FadeMode CurrentFadeMode => _currentFadeMode;

    public event Action<FadeMode> FadeIsExecuted;
    public event Action<FadeMode> FadeStopped;
    public event Action FadeIsCanceled;

    private void Awake()
    {
        if (!gameObject.TryGetComponent(out _fadeImage))
        {
            _fadeImage = gameObject.GetComponentInParent<Image>(true);

            if (_fadeImage == null)
                _fadeImage = gameObject.AddComponent<Image>();

            _fadeImage.color = Mathf.Approximately(_fadeImage.color.a, 1.0f) ? _fadeImage.color : new Color(_fadeImage.color.r, _fadeImage.color.g, _fadeImage.color.b, 1.0f);
        }

        if (!gameObject.TryGetComponent(out _canvas))
        {
            _canvas = gameObject.GetComponentInParent<Canvas>(true);

            if (_canvas == null)
                gameObject.AddComponent<Canvas>();
        }

        _opaqueColor = _fadeImage.color;
        _fadeImage.color = Color.clear;
        _fadeImage.raycastTarget = false;
        _canvas.enabled = false;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        _activeFadeCTS?.Dispose();
        _activeFadeCTS = null;
    }

    public void CancelFade()
    {
        if (!IsFading)
            return;

        _activeFadeCTS?.Cancel();
    }

    public void StartFadeCoroutine(FadeMode fadeMode) => StartFadeCoroutine(fadeMode, _defaultFadeDuration);
    public void StartFadeCoroutine(FadeMode fadeMode, float fadeDuration)
    {
        if (_activeFadeCoroutine != null)
            StopCoroutine(_activeFadeCoroutine);

        _activeFadeCoroutine = StartCoroutine(FadeCoroutine(fadeMode, fadeDuration));
    }

    private IEnumerator FadeCoroutine(FadeMode fadeMode, float fadeDuration)
    {
        if (fadeMode == FadeMode.None)
            yield break;

        _activeFadeCTS?.Cancel();
        _activeFadeCTS = new CancellationTokenSource();
        var ct = _activeFadeCTS.Token;

        _currentFadeMode = fadeMode;
        IsFading = true;

        var fadeImageColor = _fadeImage.color;
        fadeImageColor.a = fadeMode == FadeMode.FadeIn ? _opaqueColor.a : 0.0f;
        _fadeImage.color = fadeImageColor;

        _canvas.enabled = true;

        float startAlpha = fadeImageColor.a;
        float endAlpha = 1.0f - startAlpha;
        float inverseFadeDuration = 1.0f / fadeDuration;

        for (float elapsedTime = 0.0f; elapsedTime < fadeDuration; elapsedTime += Time.unscaledDeltaTime)
        {
            if (ct.IsCancellationRequested)
            {
                FadeIsCanceled?.Invoke();
                break;
            }

            fadeImageColor.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime * inverseFadeDuration);
            _fadeImage.color = fadeImageColor;
            yield return null;
        }

        fadeImageColor.a = ct.IsCancellationRequested ? startAlpha : endAlpha;
        _fadeImage.color = fadeImageColor;

        _canvas.enabled = false;
        IsFading = false;
        _activeFadeCTS?.Dispose();
        _activeFadeCTS = null;
    }

    public Task FadeAsync(FadeMode fadeMode) => FadeAsync(fadeMode, _defaultFadeDuration);
    public async Task FadeAsync(FadeMode fadeMode, float fadeDuration)
    {
        if (fadeMode == FadeMode.None)
            return;

        _activeFadeCTS?.Cancel();
        _activeFadeCTS = new CancellationTokenSource();
        var ct = _activeFadeCTS.Token;

        _currentFadeMode = fadeMode;
        IsFading = true;

        var fadeImageColor = _fadeImage.color;
        fadeImageColor.a = fadeMode == FadeMode.FadeIn ? _opaqueColor.a : 0f;
        _fadeImage.color = fadeImageColor;

        _canvas.enabled = true;

        try
        {
            float startAlpha = fadeImageColor.a;
            float endAlpha = 1f - startAlpha;
            float inverseDuration = 1f / fadeDuration;

            for (float elapsed = 0f; elapsed < fadeDuration; elapsed += Time.deltaTime)
            {
                ct.ThrowIfCancellationRequested();

                fadeImageColor.a = Mathf.Lerp(startAlpha, endAlpha, elapsed * inverseDuration);
                _fadeImage.color = fadeImageColor;
                await Task.Yield();
            }

            fadeImageColor.a = endAlpha;
            _fadeImage.color = fadeImageColor;
        }
        catch (OperationCanceledException)
        {
            FadeIsCanceled?.Invoke();
            fadeImageColor.a = _currentFadeMode == FadeMode.FadeIn ? _opaqueColor.a : 0.0f;
            _fadeImage.color = fadeImageColor;
        }
        finally
        {
            _canvas.enabled = false;
            IsFading = false;
            _activeFadeCTS?.Dispose();
            _activeFadeCTS = null;
        }
    }
}
