using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIImageSpriteLooper : MonoBehaviour
{
    private const uint MAX_LOOP_COUNT = 50;

    [SerializeField]
    private Sprite[] _spriteVariants = default;

    private uint _maxLoopCount = 1;

    private Image _image = default;
    private Sprite _defaultSprite = default;
    private Color _defaultColor = default;

    private CancellationTokenSource _cts;

    public event Action LoopStarted;
    public event Action LoopEnded;
    public event Action LoopStopped;
    public event Action ReachedEndOfArray;
    public event Action ReachedStartOfArray;

    public bool IsDefaultImage => _image.sprite == _defaultSprite;
    public Image Image => _image;
    public Color DefaultColor => _defaultColor;
    public Sprite DefaultSprite => _defaultSprite;
    public Sprite CurrentSprite => _image.sprite;

    #region Unity Lifecycle Methods
    private void Awake()
    {
        _image = _image != null ? _image : GetComponent<Image>();

        _defaultSprite = _image.sprite;
        _defaultColor = _image.color;
    }

    private void OnEnable()
    {
        LoopStopped += OnLoopStopped;
    }

    private void Start()
    {
        _image.sprite = _image.sprite != DefaultSprite ? DefaultSprite : _image.sprite;
    }

    private void OnDisable()
    {
        LoopStopped -= OnLoopStopped;
    }
    #endregion

    #region Public Methods
    public void SetImageColor(Color newColor)
    {
        if (newColor == _image.color)
            return;

        _image.color = newColor;
    }

    public async void StartLoop(float delayBetweenSprites, uint maxLoops, Func<bool> stopCondition = null)
    {
        StopLoop();

        if (maxLoops <= 0 || maxLoops > MAX_LOOP_COUNT)
        {
            Debug.LogErrorFormat("The number of maximum loops is invalid!");
            return;
        }

        _cts = new();
        _maxLoopCount = maxLoops;
        LoopStarted?.Invoke();

        try
        {
            await AnimateSprites(delayBetweenSprites, _cts.Token, stopCondition);
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
    #endregion

    #region Private Methods
    private async Task AnimateSprites(float delayBetweenSprites, CancellationToken cancellationToken, Func<bool> stopCondition = null)
    {
        if (_spriteVariants == null || _spriteVariants.Length == 0)
            return;

        uint currentLoopCount = 0;

        do
        {
            for (int i = 0; i < _spriteVariants.Length; i++)
            {
                if (CheckStopCondition(cancellationToken, stopCondition))
                    break;

                await UpdateSprite(_spriteVariants[i], delayBetweenSprites, cancellationToken);
            }

            ReachedEndOfArray?.Invoke();

            for (int i = _spriteVariants.Length - 1; i >= 0; i--)
            {
                if (CheckStopCondition(cancellationToken, stopCondition))
                    break;

                await UpdateSprite(_spriteVariants[i], delayBetweenSprites, cancellationToken);
            }

            ReachedStartOfArray?.Invoke();

            ++currentLoopCount;

        } while (currentLoopCount < _maxLoopCount && !cancellationToken.IsCancellationRequested);
    }

    /// <summary>
    /// Checks if the stop condition is met.
    /// </summary>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <param name="stopCondition">Optional function to determine if the loop should stop.</param>
    /// <returns>True if the stop condition is met, otherwise false.</returns>
    private bool CheckStopCondition(CancellationToken cancellationToken, Func<bool> stopCondition = null)
    {
        return cancellationToken.IsCancellationRequested || (stopCondition != null && stopCondition());
    }

    private async Task UpdateSprite(Sprite sprite, float delayBetweenSprites, CancellationToken cancellationToken)
    {
        _image.sprite = sprite;
        await Task.Delay((int)(delayBetweenSprites * 1000.0f), cancellationToken); // Convert seconds to milliseconds
    }

    private void OnLoopStopped()
    {
        _image.sprite = _image.sprite != DefaultSprite ? DefaultSprite : _image.sprite;

        _image.color = _image.color != DefaultColor ? DefaultColor : _image.color;
    }
    #endregion
}
