using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UIImageSpriteLooper : MonoBehaviour
{
    [SerializeField]
    private Sprite[] _spriteVariants = default;

    private Image _image = default;
    private Sprite _defaultSprite = default;
    private Color _defaultColor = Color.white;

    private CancellationTokenSource _cts;

    public event Action LoopStarted;
    public event Action LoopEnded;
    public event Action LoopStopped;

    public bool IsDefaultImage => _image.sprite == _defaultSprite;
    public bool IsDefaultColor => _image.color == _defaultColor;
    public Sprite DefaultSprite => _defaultSprite;
    public Sprite CurrentSprite => _image.sprite;
    public Color DefaultColor => _defaultColor;
    public Color CurrentColor => _image.color;

    private void Awake()
    {
        _image = _image != null ? _image : GetComponent<Image>();

        _defaultColor = _image.color;
        _defaultSprite = _image.sprite;
    }

    private void OnEnable()
    {
        if (_image.sprite != _defaultSprite)
            _image.sprite = _defaultSprite;

        if (_image.color != _defaultColor)
            _image.color = _defaultColor;
    }

    public void ChangeColor(Color newColor)
    {
        _image.color = newColor;
    }

    public async void StartLoop(float delayBetweenSprites, bool loopOnce, Color? newColor = null, Func<bool> stopCondition = null)
    {
        StopLoop();

        _cts = new();
        LoopStarted?.Invoke();
        Color colorToUse = (Color)(newColor == null ? _defaultColor : newColor);

        try
        {
            await AnimateSprites(delayBetweenSprites, loopOnce, _cts.Token, colorToUse, stopCondition);
        }
        catch (OperationCanceledException)
        {
            Debug.LogFormat("{0}'s {1} <color=white>has been stopped</color> on.", gameObject.name, this);
        }
        finally
        {
            LoopEnded?.Invoke();
            _cts?.Dispose();
            _cts = null;
        }
    }

    public void StopLoop()
    {
        LoopStopped?.Invoke();
        _cts?.Cancel();
    }

    private async Task AnimateSprites(float delayBetweenSprites, bool loopOnce, CancellationToken cancellationToken, Color newColor, Func<bool> stopCondition = null)
    {
        if (_spriteVariants == null || _spriteVariants.Length == 0)
            return;

        do
        {
            for (int i = 0; i < _spriteVariants.Length; i++)
            {
                if (CheckStopCondition(cancellationToken, stopCondition))
                    break;

                await UpdateSprite(_spriteVariants[i], delayBetweenSprites, cancellationToken);
            }

            if (newColor != _defaultColor)
                _image.color = newColor;

            for (int i = _spriteVariants.Length - 1; i >= 0; i--)
            {
                if (CheckStopCondition(cancellationToken, stopCondition))
                    break;

                await UpdateSprite(_spriteVariants[i], delayBetweenSprites, cancellationToken);
            }

            if (_image.color != _defaultColor)
                _image.color = _defaultColor;

        } while (!loopOnce && !cancellationToken.IsCancellationRequested);
    }

    private bool CheckStopCondition(CancellationToken cancellationToken, Func<bool> stopCondition = null)
    {
        return cancellationToken.IsCancellationRequested || (stopCondition != null && stopCondition());
    }

    private async Task UpdateSprite(Sprite sprite, float delayBetweenSprites, CancellationToken cancellationToken)
    {
        _image.sprite = sprite;
        await Task.Delay((int)(delayBetweenSprites * 1000.0f), cancellationToken); // Convert seconds to milliseconds
    }

    //public async void ChangeColorForMilliseconds(Color newColor, int delayTime)
    //{
    //    if (_isChangingColor)
    //        return;

    //    _isChangingColor = true;
    //    Color defaultColor = _image.color;
    //    ChangeColor(newColor);

    //    await Task.Delay(delayTime);

    //    ChangeColor(defaultColor);
    //    _isChangingColor = false;
    //}

    //public async void SwitchImageForMilliseconds(float delayTime)
    //{
    //    if (_isChangingImage)
    //        return;

    //    _isChangingImage = true;
    //    SwitchImage();

    //    await Task.Delay((int)delayTime * 1000); // Convert seconds to milliseconds

    //    SwitchImage();
    //    _isChangingImage = false;
    //}

    //public async void SwitchImageAndChangeColorForMilliseconds(Color newColor, float delayTime)
    //{
    //    if (_isChangingImage || _isChangingColor)
    //        return;

    //    _isChangingImage = true;
    //    _isChangingColor = true;

    //    Color defaultColor = _image.color;
    //    SwitchImage();
    //    ChangeColor(newColor);

    //    await Task.Delay((int)delayTime * 1000); // Convert seconds to milliseconds

    //    ChangeColor(defaultColor);
    //    SwitchImage();
    //    _isChangingImage = false;
    //    _isChangingColor = false;
    //}
}
