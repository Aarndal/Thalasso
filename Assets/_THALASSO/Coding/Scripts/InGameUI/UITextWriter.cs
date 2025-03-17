using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class UITextWriter : MonoBehaviour
{
    [Flags]
    public enum TypeWriterMethods
    {
        ShowText = 1 << 0,
        WipeText = 1 << 1,
        DeleteText = 1 << 2 | WipeText,
    }

    [Flags]
    public enum FadeMethods
    {
        FadeIn = 1 << 0,
        FadeOut = 1 << 1,
    }

    [SerializeField, Min(0.001f), Tooltip("In Seconds.")]
    private float _fadeDuration = 2.0f;
    [SerializeField, Min(0.0f), Tooltip("In Seconds.")]
    private float _startDelay = 0.5f;

    private int _defaultMaxVisibleCharacters = default;

    private TMP_Text _tmpText = default;

    private readonly HashSet<CancellationTokenSource> _cts = new();

    #region Unity Lifecycle Methods
    private void Awake()
    {
        _tmpText = _tmpText != null ? _tmpText : GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        _tmpText.enabled = false;
    }

    private void OnDisable()
    {
        foreach (var cts in _cts)
        {
            cts?.Cancel();
            cts?.Dispose();
        }

        _cts.Clear();
    }
    #endregion

    #region Public Methods
    public async void TypeWriter(TypeWriterMethods usedMethod, float startDelay, float delayBetweenLetters, float delayBetweenWords, bool typeFromLeftToRight = true, string text = null)
    {
        CancellationTokenSource cts = new();
        _cts.Add(cts);
        _tmpText.enabled = true;
        SetAlpha(1.0f);

        _defaultMaxVisibleCharacters = _tmpText.maxVisibleCharacters;
        _tmpText.maxVisibleCharacters = 0;

        if (text != null)
            _tmpText.text = text;

        try
        {
            if ((TypeWriterMethods.ShowText & usedMethod) != 0)
                await ShowText(cts.Token, startDelay, delayBetweenLetters, delayBetweenWords, typeFromLeftToRight);

            if ((TypeWriterMethods.WipeText & usedMethod) != 0)
                await WipeText(cts.Token, startDelay, delayBetweenLetters, delayBetweenWords, typeFromLeftToRight);

            if ((TypeWriterMethods.DeleteText & usedMethod) != 0)
                await DeleteText(cts.Token, startDelay, delayBetweenLetters, delayBetweenWords, typeFromLeftToRight);
        }
        catch (OperationCanceledException)
        {
            Debug.LogErrorFormat("{0}'s {1} <color=white>has been stopped</color> on.", gameObject.name, this);
        }
        finally
        {
            _cts.Remove(cts);
            cts?.Dispose();

            _tmpText.maxVisibleCharacters = _defaultMaxVisibleCharacters;

            if (_cts.Count == 0 && _tmpText.enabled)
                _tmpText.enabled = false;
        }
    }

    public async void Fade(FadeMethods usedMethod, float startDelay, float fadeDuration, string text = null)
    {
        CancellationTokenSource cts = new();
        _cts.Add(cts);
        _tmpText.enabled = true;
        SetAlpha(0.0f);

        if (text != null)
            _tmpText.text = text;

        try
        {
            if ((FadeMethods.FadeIn & usedMethod) != 0)
                await FadeIn(cts.Token, startDelay, fadeDuration);

            if ((FadeMethods.FadeOut & usedMethod) != 0)
                await FadeOut(cts.Token, startDelay, fadeDuration);
        }
        catch (OperationCanceledException)
        {
            Debug.LogFormat("{0}'s {1} <color=white>has been stopped</color> on.", gameObject.name, this);
        }
        finally
        {
            _cts.Remove(cts);
            cts?.Dispose();

            if (_cts.Count == 0 && _tmpText.enabled)
                _tmpText.enabled = false;
        }
    }
    #endregion

    #region Private Fade Methods
    private async Task FadeIn(CancellationToken cancellationToken, float startDelay, float fadeDuration)
    {
        await Task.Delay(TimeSpan.FromSeconds(startDelay), cancellationToken);

        float elapsedTime = 0.0f;

        while (elapsedTime < fadeDuration)
        {
            cancellationToken.ThrowIfCancellationRequested();

            elapsedTime += Time.deltaTime;
            float alphaValue = Mathf.Clamp01(elapsedTime / fadeDuration);
            SetAlpha(alphaValue);

            await Task.Yield();
        }

        SetAlpha(1.0f);
    }

    private async Task FadeOut(CancellationToken cancellationToken, float startDelay, float fadeDuration)
    {
        await Task.Delay(TimeSpan.FromSeconds(startDelay), cancellationToken);

        float elapsedTime = 0.0f;

        while (elapsedTime < fadeDuration)
        {
            cancellationToken.ThrowIfCancellationRequested();

            elapsedTime += Time.deltaTime;
            float alphaValue = Mathf.Clamp01(1 - elapsedTime / fadeDuration);
            SetAlpha(alphaValue);

            await Task.Yield();
        }

        SetAlpha(0.0f);
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
    #endregion

    #region Private TextWriter Methods
    private async Task ShowText(CancellationToken cancellationToken, float startDelay, float delayBetweenLetters, float delayBetweenWords, bool fromLeftToRight = true)
    {
        await Task.Delay(TimeSpan.FromSeconds(startDelay), cancellationToken);

        for (int i = 0; i < _tmpText.text.Length; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _tmpText.maxVisibleCharacters = i + 1;

            if (_tmpText.text[i] == ' ')
                await Task.Delay(TimeSpan.FromSeconds(delayBetweenWords), cancellationToken);
            else
                await Task.Delay(TimeSpan.FromSeconds(delayBetweenLetters), cancellationToken);
        }
    }

    private async Task DeleteText(CancellationToken cancellationToken, float startDelay, float delayBetweenLetters, float delayBetweenWords, bool fromLeftToRight = true)
    {
        await Task.Delay(TimeSpan.FromSeconds(startDelay), cancellationToken);

        StringBuilder stringBuilder = new(_tmpText.text);

        for (int i = 0; i < stringBuilder.Capacity; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (fromLeftToRight)
                stringBuilder.Remove(0, 1);
            else
                stringBuilder.Remove(stringBuilder.Length - 1, 1);

            _tmpText.text = stringBuilder.ToString();

            await Task.Delay(TimeSpan.FromSeconds(delayBetweenLetters), cancellationToken);
        }
    }

    private async Task WipeText(CancellationToken cancellationToken, float startDelay, float delayBetweenLetters, float delayBetweenWords, bool fromLeftToRight = true)
    {
        await Task.Delay(TimeSpan.FromSeconds(startDelay), cancellationToken);

        StringBuilder stringBuilder = new(_tmpText.text);

        for (int i = 0; i < stringBuilder.Capacity; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (fromLeftToRight)
            {
                stringBuilder[i] = ' ';
            }
            else
            {
                if (stringBuilder.Length - 1 - i >= 0)
                    stringBuilder[stringBuilder.Length - 1 - i] = ' ';
            }

            _tmpText.text = stringBuilder.ToString();

            await Task.Delay(TimeSpan.FromSeconds(delayBetweenLetters), cancellationToken);
        }
    }
    #endregion

}
