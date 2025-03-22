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
    private readonly Dictionary<AkGameObj, HashSet<uint>> _soundEmitter = new();

    public uint ID => _wwiseEvent.Id;
    public bool IsOneTimeEvent => _isOneTimeEvent;
    public bool IsValid => IsWwiseEventValid();
    public string EventName => _wwiseEvent.Name;
    public AK.Wwise.Event Value => _wwiseEvent;


    private void Awake()
    {
        IsWwiseEventValid();

        _playingIDs = new uint[_maxPlayingID];
    }

    private void OnDisable()
    {
        foreach (var cts in _cts)
        {
            cts?.Cancel();
        }

        _cts.Clear();
    }

    private void OnDestroy()
    {
        Stop();
    }

    /// <summary>
    /// Checks if the WwiseEvent is currently playing on the given Emitter Object.
    /// </summary>
    /// <param name="akGameObject">Emitter Object</param>
    /// <returns></returns>
    public bool IsPlayingOn(AkGameObj akGameObject)
    {
        if (!IsValid)
            return false;

        if (!IsAkGameObjectValid(akGameObject))
            return false;

        if (_soundEmitter.ContainsKey(akGameObject))
            return true;

        AkUnitySoundEngine.GetPlayingIDsFromGameObject(akGameObject.gameObject, ref _maxPlayingID, _playingIDs);

        if (_playingIDs.Length == 0)
        {
            Debug.LogFormat("There is no WwiseEvent playing on {0}.", akGameObject.gameObject.name);
            return false;
        }

        HashSet<uint> myPlayingIDs = _playingIDs.Where((playingID) => AkUnitySoundEngine.GetEventIDFromPlayingID(playingID) == ID).ToHashSet();

        if (myPlayingIDs.Count == 0)
        {
            Debug.LogFormat("{1} is currently not playing on {0}.", akGameObject.gameObject.name, EventName);
            return false;
        }

        _soundEmitter.TryAdd(akGameObject, myPlayingIDs);
        return true;
    }

    /// <summary>
    /// Plays the WwiseEvent on the given Emitter Object.
    /// </summary>
    /// <param name="akGameObject">Emitter Object</param>
    /// <returns></returns>
    public bool Play(AkGameObj akGameObject)
    {
        if (!IsValid)
            return false;

        if (!IsAkGameObjectValid(akGameObject))
            return false;

        uint playingID = _wwiseEvent.Post(akGameObject.gameObject);

        if (_soundEmitter.ContainsKey(akGameObject))
        {
            _soundEmitter[akGameObject].Add(playingID);
            return true;
        }

        HashSet<uint> myPlayingIDs = new() { playingID };
        _soundEmitter.TryAdd(akGameObject, myPlayingIDs);
        return true;
    }

    /// <summary>
    /// Plays the WwiseEvent on the given Emitter Object after a certain time in seconds. For an input below 0.1 seconds, there will be no delay.
    /// </summary>
    /// <param name="akGameObject">Emitter Object</param>
    /// <param name="delayInSeconds"></param>
    /// <returns></returns>
    public async Task<bool> PlayWithDelay(AkGameObj akGameObject, float delayInSeconds)
    {
        if (delayInSeconds < 0.0f)
            return false;

        if (delayInSeconds >= 0.1f)
        {
            if (!await DelaySound(akGameObject.gameObject, delayInSeconds))
                return false;
        }

        return Play(akGameObject);
    }

    /// <summary>
    /// Stops the WwiseEvent on the given Emitter Object.
    /// </summary>
    /// <param name="akGameObject">Emitter Object</param>
    /// <returns></returns>
    public bool Stop(AkGameObj akGameObject)
    {
        if (!IsValid)
            return false;

        if (!IsAkGameObjectValid(akGameObject))
            return false;

        _wwiseEvent.Stop(akGameObject.gameObject);
        _soundEmitter.Remove(akGameObject);
        return true;
    }

    /// <summary>
    /// Stops all instances of the WwiseEvent on all Emitter Objects.
    /// </summary>
    /// <returns></returns>
    public bool Stop()
    {
        if (!IsValid)
            return false;

        AkUnitySoundEngine.ExecuteActionOnEvent(ID, AkActionOnEventType.AkActionOnEventType_Stop);
        _soundEmitter.Clear();
        return true;
    }

    /// <summary>
    /// Used to switch between WwiseEvents on the given Emitter Object. If no WwiseEvent is provided, it stops all current WwiseEvents on the Emitter Object.
    /// </summary>
    /// <param name="akGameObject">Emitter Object</param>
    /// <param name="wwiseEvent"></param>
    /// <returns></returns>
    public bool Switch(AkGameObj akGameObject, SO_WwiseEvent wwiseEvent = null) //TO-DO: Needs renaming for clarity!
    {
        if (!IsAkGameObjectValid(akGameObject))
            return false;

        if (wwiseEvent == null)
        {
            AkUnitySoundEngine.StopAll(akGameObject.gameObject);
            _soundEmitter.Remove(akGameObject);
        }

        if (wwiseEvent != null && wwiseEvent.IsValid)
            wwiseEvent.Stop(akGameObject);

        return Play(akGameObject);
    }

    /// <summary>
    /// Checks whether anything in the Wwise-Type Event property drawer has been selected. 
    /// </summary>
    /// <returns></returns>
    private bool IsWwiseEventValid()
    {
        if (!_wwiseEvent.IsValid())
        {
            _wwiseEvent.Validate();
            return false;
        }

        if (_wwiseEvent == null)
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
