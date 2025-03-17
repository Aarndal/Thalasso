using System;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class UITextFade : MonoBehaviour
{
    [SerializeField, Min(0.001f), Tooltip("In Seconds.")]
    private float _fadeDuration = 2.0f;
    [SerializeField, Min(0.0f), Tooltip("In Seconds.")]
    private float _startDelay = 0.5f;

    private TMP_Text _tmpText = default;
    //private TextMeshPro _tmPro = default;

    private CancellationTokenSource _cts;

    private void Awake()
    {
        _tmpText = _tmpText != null ? _tmpText : GetComponent<TMP_Text>();
    }

    private void Start()
    {
        SetAlpha(0.0f);
        _tmpText.enabled = false;
    }

    private void OnDestroy()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }

    public async void Fade(string text = null)
    {
        _cts = new();
        _tmpText.enabled = true;
        SetAlpha(0.0f);

        if (text != null)
        {
            _tmpText.text = text;
        }

        try
        {
            await FadeIn(_cts.Token);
            await FadeOut(_cts.Token);
        }
        catch (OperationCanceledException)
        {
            Debug.LogFormat("{0}'s {1} <color=white>has been stopped</color> on.", gameObject.name, this);
        }
        finally
        {
            _cts?.Dispose();
            _cts = null;
        }
    }

    private async Task FadeOut(CancellationToken cancellationToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(_startDelay), cancellationToken);

        float elapsedTime = 0.0f;

        while (elapsedTime < _fadeDuration)
        {
            cancellationToken.ThrowIfCancellationRequested();

            elapsedTime += Time.deltaTime;
            float alphaValue = Mathf.Clamp01(1 - elapsedTime / _fadeDuration);
            SetAlpha(alphaValue);

            await Task.Yield();
        }

        SetAlpha(0.0f);
    }

    private async Task FadeIn(CancellationToken cancellationToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(_startDelay), cancellationToken);

        float elapsedTime = 0.0f;

        while (elapsedTime < _fadeDuration)
        {
            cancellationToken.ThrowIfCancellationRequested();

            elapsedTime += Time.deltaTime;
            float alphaValue = Mathf.Clamp01(elapsedTime / _fadeDuration);
            SetAlpha(alphaValue);

            await Task.Yield();
        }

        SetAlpha(1.0f);
    }

    private void SetAlpha(float alphaValue)
    {
        if (alphaValue <= 0.001f)
            alphaValue = 0.0f;

        if (alphaValue >= 0.999f)
            alphaValue = 1.0f;

        Color tmp = _tmpText.color;
        tmp.a = alphaValue;
        _tmpText.color = tmp;
    }
}
