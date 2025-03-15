using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WwiseHelper
{
#if WWISE_2024_OR_LATER
    [RequireComponent(typeof(AkGameObj))]
#endif
    public class WwiseEventResponder : Responder
    {
#if WWISE_2024_OR_LATER
        [Space(5)]

        [Header("Wwise Events Settings")]
        [SerializeField]
        protected SO_WwiseEvent[] _wwiseEvents = default;
        [SerializeField]
        protected bool _playOnOtherObject = false;
        [SerializeField]
        protected bool _isEnvironmentAware = false;
        [SerializeField]
        protected bool _isRoomAware = false;

        protected ResponderState _currentState = ResponderState.None;

        protected GameObject _eventReceiver = default;
        protected Rigidbody _rigidbody = default;

        protected AkGameObj _akGameObject = default;
        protected AkRoomAwareObject _akRoomAwareObject = default;

        public ResponderState CurrentState { get => _currentState; protected set => _currentState = value; }

        public readonly Dictionary<string, SO_WwiseEvent> AudioEvents = new();
        public readonly HashSet<SO_WwiseEvent> PlayedOneTimeAudioEvents = new();
#endif

        protected override void Awake()
        {
            base.Awake();

#if WWISE_2024_OR_LATER
            CurrentState = _startState;

            InitAudioEvents();
            SetEventReceiver(gameObject);
            InitRequiredWwiseComponents();
#endif
        }

        public override void Respond(GameObject triggeringObject, ResponderState responderState)
        {
#if WWISE_2024_OR_LATER

            if (_playOnOtherObject)
            {
                if (_eventReceiver != triggeringObject)
                {
                    SetEventReceiver(triggeringObject);
                    InitRequiredWwiseComponents();
                }
            }

            if (responderState == ResponderState.Switch)
            {
                responderState = CurrentState == ResponderState.TurnOff ? ResponderState.TurnOn : ResponderState.TurnOff;
            }

            switch (responderState)
            {
                case ResponderState.None:
                    Debug.LogWarningFormat("<color=yellow>No valid ResponderState set</color> for Trigger activated by <color=cyan>{0}</color>", triggeringObject);
                    return;
                case ResponderState.TurnOff:
                    TurnOff();
                    break;
                case ResponderState.TurnOn:
                    TurnOn();
                    break;
                default:
                    break;
            }
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

        protected void TurnOff()
        {
#if WWISE_2024_OR_LATER
            CurrentState = ResponderState.TurnOff;

            foreach (var audioEvent in AudioEvents.Values)
            {
                if (audioEvent.IsPlayingOn(_akGameObject))
                    audioEvent.Stop(_akGameObject);
            }
#endif
        }

        protected void TurnOn()
        {
#if WWISE_2024_OR_LATER
            CurrentState = ResponderState.TurnOn;

            foreach (var audioEvent in AudioEvents.Values)
            {
                if (!PlayedOneTimeAudioEvents.Contains(audioEvent))
                    audioEvent.Play(_akGameObject);

                if (audioEvent.IsOneTimeEvent)
                    PlayedOneTimeAudioEvents.Add(audioEvent);
            }
#endif
        }
    }
}
