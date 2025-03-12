using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWwiseEvent", menuName = "Scriptable Objects/WwiseEvent")]
public class SO_WwiseEvent : ScriptableObject
{
    [SerializeField]
    private AK.Wwise.Event _wwiseEvent = default;

    private uint _maxPlayingID = 50; // Maximum number of PlayingIDs to search for on one GameObject.
    private uint[] _playingIDs;

    public uint ID => _wwiseEvent.Id;
    public bool IsValid => _wwiseEvent.IsValid();
    public string Name => _wwiseEvent.Name;
    public AK.Wwise.Event Value => _wwiseEvent;

    private void Awake()
    {
        _playingIDs = new uint[_maxPlayingID];
    }

    /// <summary>
    /// Plays the WwiseEvent on the given GameObject.
    /// </summary>
    /// <param name="akGameObject"></param>
    /// <returns></returns>
    public bool Post(AkGameObj akGameObject)
    {
        if (_wwiseEvent.IsValid())
        {
            _wwiseEvent?.Post(akGameObject.gameObject);
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
            return true;
        }
        return false;
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
}
