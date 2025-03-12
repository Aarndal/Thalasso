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
        protected AK.Wwise.Event[] _wwiseEvents = default;
        [SerializeField]
        protected bool _playOnOtherObject = false;
        [SerializeField]
        protected bool _isEnvironmentAware = false;
        [SerializeField]
        protected bool _isRoomAware = false;

        [Header("References for Wwise Events")]
        [SerializeField]
        protected GameObject _eventReceiver = default;
        [SerializeField]
        protected Rigidbody _rigidbody = default;
        
        protected AkGameObj _akGameObject = default;
        protected AkRoomAwareObject _akRoomAwareObject = default;

        public readonly Dictionary<string, AK.Wwise.Event> AudioEvents = new();
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
                    if (!AudioEvents.TryAdd(wwiseEvent.Name, wwiseEvent))
                    {
                        Debug.LogWarningFormat("Wwise Event color=cyan>{0}</color> <color=yellow>is defined multiple times</color> in <color=cyan>{1}</color>'s {2} component!", wwiseEvent.Name, gameObject.name, this);
                    }
                }
            }
#endif
        }

        protected void InitRequiredWwiseComponents()
        {
#if WWISE_2024_OR_LATER
            // Interactions between AkGameObj/AkRoomAwareObject and AkEnvironment/AkRoom require a Rigidbody component on either the EventReceiver or the environment/room.
            if (!_eventReceiver.TryGetComponent(out _rigidbody))
            {
                if (_rigidbody == null)
                {
                    _rigidbody = _eventReceiver.AddComponent<Rigidbody>();
                    _rigidbody.isKinematic = true;
                }

                if (_rigidbody.gameObject != _eventReceiver)
                {
                    if (!_rigidbody.gameObject.TryGetComponent(out _akGameObject))
                        _akGameObject = _rigidbody.gameObject.AddComponent<AkGameObj>();

                    _eventReceiver = _akGameObject.gameObject;
                }
            }

            _akGameObject.isEnvironmentAware = _isEnvironmentAware;

            if (_isRoomAware)
            {
                if (!_eventReceiver.TryGetComponent(out _akRoomAwareObject))
                {
                    _akRoomAwareObject = _eventReceiver.AddComponent<AkRoomAwareObject>();
                }
            }
#endif
        }

        protected void SetEventReceiver(GameObject gameObject)
        {
#if WWISE_2024_OR_LATER
            if (_playOnOtherObject && gameObject != null)
            {
                _eventReceiver = gameObject;

                if (!_eventReceiver.TryGetComponent(out _akGameObject))
                    _akGameObject = _eventReceiver.AddComponent<AkGameObj>();

                return;
            }

            _eventReceiver = this.gameObject;

            if (!_eventReceiver.TryGetComponent(out _akGameObject))
            {
                _akGameObject = _eventReceiver.AddComponent<AkGameObj>();
            }
#endif
        }
    }
}
