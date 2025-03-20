using System;
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

    private readonly HashSet<CancellationTokenSource> _cts = new();

    public uint ID => _wwiseEvent.Id;
    public bool IsOneTimeEvent => _isOneTimeEvent;
    public bool IsValid => IsWwiseEventValid(_wwiseEvent);
    public string EventName => _wwiseEvent.Name;
    public AK.Wwise.Event Value => _wwiseEvent;
    public readonly HashSet<GameObject> PlayingSources = new();

    private void Awake()
    {
        IsWwiseEventValid(_wwiseEvent);

        _playingIDs = new uint[_maxPlayingID];
    }

    private void OnDisable()
    {
        foreach (var cts in _cts)
            cts?.Cancel();

        _cts.Clear();
    }

    /// <summary>
    /// Checks if the WwiseEvent is currently playing on the given GameObject.
    /// </summary>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    public bool IsPlayingOn(AkGameObj akGameObject)
    {
        if (!IsValid)
            return false;

        if (!IsAkGameObjectValid(akGameObject))
            return false;

        AkUnitySoundEngine.GetPlayingIDsFromGameObject(akGameObject.gameObject, ref _maxPlayingID, _playingIDs);

        if (_playingIDs.Length == 0)
        {
            Debug.LogFormat("There is no WwiseEvent playing on {0}.", akGameObject.gameObject.name);
            return false;
        }

        if (!_playingIDs.Any((id) => AkUnitySoundEngine.GetEventIDFromPlayingID(id) == ID))
        {
            Debug.LogFormat("{1} is currently not playing on {0}.", akGameObject.gameObject.name, EventName);
            return false;
        }

        if (!PlayingSources.Contains(akGameObject.gameObject))
            PlayingSources.Add(akGameObject.gameObject);

        return true;
    }

    /// <summary>
    /// Plays the WwiseEvent on the given GameObject.
    /// </summary>
    /// <param name="akGameObject"></param>
    /// <returns></returns>
    public bool Play(AkGameObj akGameObject)
    {
        if (!IsValid)
            return false;

        if (!IsAkGameObjectValid(akGameObject))
            return false;

        _wwiseEvent?.Post(akGameObject.gameObject);
        PlayingSources.Add(akGameObject.gameObject);

        return true;
    }

    /// <summary>
    /// Plays the WwiseEvent on the given GameObject after a certain time in seconds. For an input of 0.1 seconds or below, there will be no delay.
    /// </summary>
    /// <param name="akGameObject"></param>
    /// <param name="delayInSeconds"></param>
    /// <returns></returns>
    public async Task<bool> PlayWithDelay(AkGameObj akGameObject, float delayInSeconds)
    {
        if (delayInSeconds < 0.0f)
            return false;

        if (delayInSeconds > 0.1f)
        {
            if (!await DelaySound(akGameObject.gameObject, delayInSeconds))
                return false;
        }

        return Play(akGameObject);
    }

    /// <summary>
    /// Stops the WwiseEvent on the given GameObject.
    /// </summary>
    /// <param name="akGameObject"></param>
    /// <returns></returns>
    public bool Stop(AkGameObj akGameObject)
    {
        if (!IsValid)
            return false;

        if (!IsAkGameObjectValid(akGameObject))
            return false;

        _wwiseEvent?.Stop(akGameObject.gameObject);
        PlayingSources.Remove(akGameObject.gameObject);
        return true;
    }

    /// <summary>
    /// Stops the WwiseEvent on all GameObjects.
    /// </summary>
    /// <returns></returns>
    public bool Stop()
    {
        if (!IsValid)
            return false;

        AkUnitySoundEngine.StopPlayingID(_wwiseEvent.PlayingId);
        PlayingSources.Clear();
        return true;
    }

    /// <summary>
    /// Used to switch between WwiseEvents on the given GameObject. If no WwiseEvent is provided, it stops all current WwiseEvents on the GameObject.
    /// </summary>
    /// <param name="akGameObject"></param>
    /// <param name="wwiseEvent"></param>
    /// <returns></returns>
    public bool Switch(AkGameObj akGameObject, SO_WwiseEvent wwiseEvent = null)
    {
        if (!IsAkGameObjectValid(akGameObject))
            return false;

        if (wwiseEvent == null)
        {
            AkUnitySoundEngine.StopAll(akGameObject.gameObject);
            PlayingSources.Remove(akGameObject.gameObject);
        }

        if (wwiseEvent != null && wwiseEvent.IsValid)
            wwiseEvent.Stop(akGameObject);

        return Play(akGameObject);
    }

    /// <summary>
    /// Checks whether anything in the Wwise-Type Event property drawer has been selected. 
    /// </summary>
    /// <param name="wwiseEvent"></param>
    /// <returns></returns>
    private bool IsWwiseEventValid(AK.Wwise.Event wwiseEvent)
    {
        if (!wwiseEvent.IsValid())
        {
            wwiseEvent.Validate();
            return false;
        }

        if (wwiseEvent == null)
        {
            Debug.LogErrorFormat("The referenced WwiseEvent on {0} is null!", name);
            return false;
        }

        return true;
    }

    private bool IsAkGameObjectValid(AkGameObj akGameObject)
    {
        if (akGameObject == null)
        {
            Debug.LogErrorFormat("NullReference detected by {1}: The referenced AkGameObject on {0} is null!", akGameObject.gameObject.name, name);
            return false;
        }

        return true;
    }

    private async Task<bool> DelaySound(GameObject @gameObject, float delayInSeconds)
    {
        CancellationTokenSource cts = new();
        _cts.Add(cts);

        try
        {
            await Task.Delay(TimeSpan.FromSeconds(delayInSeconds), cts.Token);
        }
        catch
        {
            Debug.LogFormat("{1} <color=white>has been stopped</color> on {0}.", @gameObject.name, EventName);
            return false;
        }
        finally
        {
            _cts.Remove(cts);
            cts?.Dispose();
        }

        return true;
    }
#endif
}
