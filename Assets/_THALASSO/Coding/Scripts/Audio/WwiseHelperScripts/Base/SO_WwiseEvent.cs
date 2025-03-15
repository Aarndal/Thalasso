using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


[CreateAssetMenu(fileName = "NewWwiseEvent", menuName = "Scriptable Objects/WwiseEvent")]
public class SO_WwiseEvent : ScriptableObject
{
#if WWISE_2024_OR_LATER
    [SerializeField]
    private AK.Wwise.Event _wwiseEvent = default;
    [SerializeField]
    private bool _isOneTimeEvent = false;

    private uint _maxPlayingID = 50; // Maximum number of PlayingIDs to search for on one GameObject.
    private uint[] _playingIDs;

    private CancellationTokenSource _cancellationTokenSource;

    public uint ID => _wwiseEvent.Id;
    public bool IsOneTimeEvent => _isOneTimeEvent;
    public bool IsValid => _wwiseEvent.IsValid();
    public string Name => _wwiseEvent.Name;
    public AK.Wwise.Event Value => _wwiseEvent;
    public readonly HashSet<GameObject> PlayingSources = new();

    private void Awake()
    {
        _playingIDs = new uint[_maxPlayingID];
    }

    private void OnDestroy()
    {
        _cancellationTokenSource?.Cancel();
    }

    /// <summary>
    /// Checks if the WwiseEvent is currently playing on the given GameObject.
    /// </summary>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    public bool IsPlayingOn(AkGameObj akGameObject)
    {
        if (_wwiseEvent.IsValid())
        {
            AkUnitySoundEngine.GetPlayingIDsFromGameObject(akGameObject.gameObject, ref _maxPlayingID, _playingIDs);

            if (_playingIDs.Length == 0)
                return false;

            if (_playingIDs.Any((id) => id == _wwiseEvent.PlayingId))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Plays the WwiseEvent on the given GameObject.
    /// </summary>
    /// <param name="akGameObject"></param>
    /// <returns></returns>
    public bool Play(AkGameObj akGameObject)
    {
        if (_wwiseEvent.IsValid())
        {
            _wwiseEvent?.Post(akGameObject.gameObject);
            PlayingSources.Add(akGameObject.gameObject);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Plays the WwiseEvent on the given GameObject after a certain time in seconds.
    /// </summary>
    /// <param name="akGameObject"></param>
    /// <param name="delayInSeconds"></param>
    /// <returns></returns>
    public async Task<bool> PlayWithDelay(AkGameObj akGameObject, float delayInSeconds)
    {
        if (delayInSeconds <= 0.0f)
            return false;

        _cancellationTokenSource = new();
        
        if (_wwiseEvent.IsValid())
        {
            if (!await DelaySound(akGameObject, delayInSeconds))
                return false;

            _wwiseEvent?.Post(akGameObject.gameObject);
            PlayingSources.Add(akGameObject.gameObject);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Stops the WwiseEvent on the given GameObject.
    /// </summary>
    /// <param name="akGameObject"></param>
    /// <returns></returns>
    public bool Stop(AkGameObj akGameObject)
    {
        if (_wwiseEvent.IsValid())
        {
            _wwiseEvent?.Stop(akGameObject.gameObject);
            PlayingSources.Remove(akGameObject.gameObject);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Stops the WwiseEvent on all GameObjects.
    /// </summary>
    /// <returns></returns>
    public bool Stop()
    {
        if (_wwiseEvent.IsValid())
        {
            AkUnitySoundEngine.StopPlayingID(_wwiseEvent.PlayingId);
            PlayingSources.Clear();
            return true;
        }
        return false;
    }

    private async Task<bool> DelaySound(AkGameObj akGameObject, float delayInSeconds)
    {
        try
        {
            await Task.Delay((int)delayInSeconds * 1000, _cancellationTokenSource.Token);
        }
        catch
        {
            Debug.LogFormat("{1} ended on {0}.", akGameObject.gameObject.name, Name);
            return false;
        }
        finally
        {
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }

        return true;
    }
#endif
}
