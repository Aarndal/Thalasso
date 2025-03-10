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
        [SerializeField]
        protected ResponderState _startState = ResponderState.TurnOff;

        [Header("Wwise Events Settings")]
        [SerializeField]
        protected AK.Wwise.Event[] _wwiseEvents = default;
        [SerializeField]
        protected bool _playOnOtherObject = false;
        [SerializeField]
        protected bool _isEnvironmentAware = false;
        [SerializeField]
        protected bool _isRoomAware = false;
        [SerializeField]
        private bool _areOneTimeEvents = false;

        protected ResponderState _currentState = ResponderState.None;

        protected GameObject _eventReceiver = default;
        protected Rigidbody _rigidbody = default;

        protected AkGameObj _akGameObject = default;
        protected AkRoomAwareObject _akRoomAwareObject = default;

        public ResponderState CurrentState { get => _currentState; protected set => _currentState = value; }

        public readonly Dictionary<string, AK.Wwise.Event> AudioEvents = new();

        protected override void Awake()
        {
            base.Awake();

            CurrentState = _startState;

            InitAudioEvents();
            SetEventReceiver(gameObject);
            InitRequiredWwiseComponents();
        }

        public override void Respond(GameObject triggeringObject, ResponderState responderState)
        {
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

            if (_areOneTimeEvents)
            {
                foreach (var triggerable in _triggers)
                {
                    if (triggerable.Interface.IsTriggerable)
                        triggerable.Interface.SwitchIsTriggerable();
                }
            }
#endif
        }

        protected void InitAudioEvents()
        {
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
        }

        protected void InitRequiredWwiseComponents()
        {
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
        }

        protected void SetEventReceiver(GameObject gameObject)
        {
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
        }

        protected void TurnOff()
        {
            CurrentState = ResponderState.TurnOff;

            foreach (var audioEvent in AudioEvents.Values)
            {
                audioEvent.Stop(_eventReceiver);
            }
            //AkUnitySoundEngine.StopAll(_eventReceiver);
        }

        protected void TurnOn()
        {
            CurrentState = ResponderState.TurnOn;

            foreach (var audioEvent in AudioEvents.Values)
            {
                audioEvent.Post(_eventReceiver);
            }
        }
    }
}
