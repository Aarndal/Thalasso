using System;
using System.Collections.Generic;
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
        GlobalEventBus.Register(GlobalEvents.UI.DiscoveredNewLocation, OnDiscoveredNewLocation);

        _tmpText.enabled = false;
    }


    private void OnDisable()
    {
        foreach (var cts in _cts)
            cts?.Cancel();

        _cts.Clear();

        GlobalEventBus.Deregister(GlobalEvents.UI.DiscoveredNewLocation, OnDiscoveredNewLocation);
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
        catch
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
        catch
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
    private async Task FadeIn(CancellationToken ct, float startDelay, float fadeDuration)
    {
        await Task.Delay(TimeSpan.FromSeconds(startDelay), ct);

        float elapsedTime = 0.0f;

        while (elapsedTime < fadeDuration && !ct.IsCancellationRequested)
        {
            elapsedTime += Time.deltaTime;
            float alphaValue = Mathf.Clamp01(elapsedTime / fadeDuration);
            SetAlpha(alphaValue);

            await Task.Yield();
        }

        SetAlpha(1.0f);
    }

    private async Task FadeOut(CancellationToken ct, float startDelay, float fadeDuration)
    {
        await Task.Delay(TimeSpan.FromSeconds(startDelay), ct);

        float elapsedTime = 0.0f;

        while (elapsedTime < fadeDuration && !ct.IsCancellationRequested)
        {
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
    private async Task ShowText(CancellationToken ct, float startDelay, float delayBetweenLetters, float delayBetweenWords, bool fromLeftToRight = true)
    {
        await Task.Delay(TimeSpan.FromSeconds(startDelay), ct);

        for (int i = 0; i < _tmpText.text.Length; i++)
        {
            if (ct.IsCancellationRequested)
                break;

            _tmpText.maxVisibleCharacters = i + 1;

            if (_tmpText.text[i] == ' ')
                await Task.Delay(TimeSpan.FromSeconds(delayBetweenWords), ct);
            else
                await Task.Delay(TimeSpan.FromSeconds(delayBetweenLetters), ct);
        }
    }

    private async Task DeleteText(CancellationToken ct, float startDelay, float delayBetweenLetters, float delayBetweenWords, bool fromLeftToRight = true)
    {
        await Task.Delay(TimeSpan.FromSeconds(startDelay), ct);

        StringBuilder stringBuilder = new(_tmpText.text);

        for (int i = 0; i < stringBuilder.Capacity; i++)
        {
            if (ct.IsCancellationRequested)
                break;

            if (fromLeftToRight)
                stringBuilder.Remove(0, 1);
            else
                stringBuilder.Remove(stringBuilder.Length - 1, 1);

            _tmpText.text = stringBuilder.ToString();

            if (_tmpText.text[i] == ' ')
                await Task.Delay(TimeSpan.FromSeconds(delayBetweenWords), ct);
            else
                await Task.Delay(TimeSpan.FromSeconds(delayBetweenLetters), ct);
        }
    }

    private async Task WipeText(CancellationToken ct, float startDelay, float delayBetweenLetters, float delayBetweenWords, bool fromLeftToRight = true)
    {
        await Task.Delay(TimeSpan.FromSeconds(startDelay), ct);

        StringBuilder stringBuilder = new(_tmpText.text);

        for (int i = 0; i < stringBuilder.Capacity; i++)
        {
            if (ct.IsCancellationRequested)
                break;

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

            if (_tmpText.text[i] == ' ')
                await Task.Delay(TimeSpan.FromSeconds(delayBetweenWords), ct);
            else
                await Task.Delay(TimeSpan.FromSeconds(delayBetweenLetters), ct);
        }
    }
    #endregion

    private void OnDiscoveredNewLocation(object[] eventArgs)
    {
        Fade((FadeMethods)eventArgs[0], (float)eventArgs[1], (float)eventArgs[2], (string)eventArgs[8]);

        TypeWriter((TypeWriterMethods)eventArgs[3], (float)eventArgs[4], (float)eventArgs[5], (float)eventArgs[6], (bool)eventArgs[7], (string)eventArgs[8]);
    }
}
