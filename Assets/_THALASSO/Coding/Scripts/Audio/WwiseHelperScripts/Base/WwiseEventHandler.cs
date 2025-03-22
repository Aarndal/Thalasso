using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WwiseHelper
{
    public abstract class WwiseEventHandler : MonoBehaviour
    {
#if WWISE_2024_OR_LATER
        [Header("Wwise Events Settings")]
        [SerializeField]
        protected SO_WwiseEvent[] _wwiseEvents = default;
        [SerializeField]
        protected bool _playOnOtherObject = false;
        [SerializeField]
        protected bool _isEnvironmentAware = false;
        [SerializeField]
        protected bool _isRoomAware = false;

        [Header("References for Wwise Events")]
        [SerializeField]
        protected GameObject _eventReceiver = default;
        
        protected AkGameObj _akGameObject = default;
        protected AkRoomAwareObject _akRoomAwareObject = default;

        public readonly Dictionary<string, SO_WwiseEvent> AudioEvents = new();
#endif

        protected virtual void Awake()
        {
#if WWISE_2024_OR_LATER
            InitAudioEvents();
            SetEventReceiver(_eventReceiver);
            InitRequiredWwiseComponents();
#endif
        }

        protected void InitAudioEvents()
        {
#if WWISE_2024_OR_LATER
            if (_wwiseEvents != null && _wwiseEvents.Length > 0)
            {
                if (!_wwiseEvents.All((o) => o != null))
                {
                    Debug.LogWarningFormat("There are <color=yellow>undefined Wwise Events</color> in <color=cyan>{0}</color>'s {1} component!", gameObject.name, this);
                }

                foreach (var wwiseEvent in _wwiseEvents)
                {
                    if (!AudioEvents.TryAdd(wwiseEvent.EventName, wwiseEvent))
                    {
                        Debug.LogWarningFormat("Wwise Event color=cyan>{0}</color> <color=yellow>is defined multiple times</color> in <color=cyan>{1}</color>'s {2} component!", wwiseEvent.EventName, gameObject.name, this);
                    }
                }
            }
#endif
        }

        protected void InitRequiredWwiseComponents()
        {
            if (_eventReceiver == null)
                return;

#if WWISE_2024_OR_LATER
            if (!_eventReceiver.TryGetComponent(out _akGameObject))
                _akGameObject = _eventReceiver.AddComponent<AkGameObj>();

            _akGameObject.isEnvironmentAware = _isEnvironmentAware;

            if (_isRoomAware && !_eventReceiver.TryGetComponent<AkRoomAwareObject>(out _))
            {
                _eventReceiver.AddComponent<AkRoomAwareObject>();
            }

            // Interactions between AkGameObj/AkRoomAwareObject and AkEnvironment/AkRoom require a Rigidbody component on either the EventReceiver or the AkEnvironment/AkRoom.
            if (_isEnvironmentAware || _isRoomAware)
            {
                if (_eventReceiver != gameObject && _eventReceiver.TryGetComponent<Rigidbody>(out _))
                    return;

                if (gameObject.TryGetComponent<Rigidbody>(out _))
                    return;

                _eventReceiver.AddComponent<Rigidbody>().isKinematic = true;
            }
#endif
        }

        protected void SetEventReceiver(GameObject gameObject)
        {
#if WWISE_2024_OR_LATER
            if (gameObject == null)
                return;

            if (_playOnOtherObject && gameObject == this.gameObject)
                return;

            _eventReceiver = gameObject;
#endif
        }
    }
}
