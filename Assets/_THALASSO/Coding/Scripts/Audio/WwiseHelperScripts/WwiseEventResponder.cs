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
        protected AK.Wwise.Event[] _wwiseEvents;
        [SerializeField]
        protected bool _playOnOtherObject = false;
        [SerializeField]
        protected bool _areEnvironmentAware = false;
        [SerializeField]
        private bool _areOneTimeEvents = false;

        protected ResponderState _currentState = ResponderState.None;

        protected GameObject _eventReceiver = default;
        protected AkGameObj _akGameObject = default;
        protected AkRoomAwareObject _akRoomAwareObject = default;
        protected Rigidbody _rigidbody = default;

        public ResponderState CurrentState { get => _currentState; protected set => _currentState = value; }

        public readonly Dictionary<string, AK.Wwise.Event> AudioEvents = new();

        protected override void Awake()
        {
            base.Awake();

            CurrentState = _startState;

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

            if (_areEnvironmentAware)
            {
                if (_akGameObject == null || !_akGameObject.gameObject.TryGetComponent(out _rigidbody))
                {
                    _rigidbody = _rigidbody != null ? _rigidbody : gameObject.GetComponentInParent<Rigidbody>();

                    if (_rigidbody == null)
                    {
                        _rigidbody = _akGameObject != null ? _akGameObject.gameObject.AddComponent<Rigidbody>() : gameObject.AddComponent<Rigidbody>();
                    }

                    if (!_rigidbody.gameObject.TryGetComponent(out _akGameObject))
                    {
                        _akGameObject = _rigidbody.gameObject.AddComponent<AkGameObj>();
                    }
                }

                if (_akGameObject != null && !_akGameObject.gameObject.TryGetComponent(out _akRoomAwareObject))
                {
                    _akRoomAwareObject = _akGameObject.gameObject.AddComponent<AkRoomAwareObject>();
                }

                _akGameObject.isEnvironmentAware = true;
            }
            else
            {
                _akGameObject = _akGameObject != null ? _akGameObject : GetComponentInParent<AkGameObj>();

                if (_akGameObject == null)
                {
                    _akGameObject = gameObject.AddComponent<AkGameObj>();
                }

                _akGameObject.isEnvironmentAware = false;
            }
        }

        protected void Start()
        {
            _eventReceiver = _akGameObject.gameObject;
        }

        public override void Respond(GameObject triggeringObject, ResponderState responderState)
        {
            if (_playOnOtherObject)
            {
                _eventReceiver = triggeringObject;

                if (!_eventReceiver.TryGetComponent(out AkGameObj akGameObject))
                {
                    akGameObject = _eventReceiver.AddComponent<AkGameObj>();
                }

                if (_areEnvironmentAware)
                    akGameObject.isEnvironmentAware = true;
                else
                    akGameObject.isEnvironmentAware = false;
            }

            if (responderState == ResponderState.None)
            {
                Debug.LogWarningFormat("<color=yellow>No valid ResponderState set</color> for Trigger activated by <color=cyan>{0}</color>", triggeringObject);
                return;
            }

            if (responderState == ResponderState.Switch)
            {
                responderState = CurrentState == ResponderState.TurnOff ? ResponderState.TurnOn : ResponderState.TurnOff;
            }

            if (responderState == ResponderState.TurnOn)
            {
                CurrentState = ResponderState.TurnOn;
                TurnOn();
            }

            if (responderState == ResponderState.TurnOff)
            {
                CurrentState = ResponderState.TurnOff;
                TurnOff();
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

        protected void TurnOff()
        {
            foreach (var audioEvent in AudioEvents.Values)
            {
                audioEvent.Stop(_eventReceiver);
            }
            //AkUnitySoundEngine.StopAll(_eventReceiver);
        }

        protected void TurnOn()
        {
            foreach (var audioEvent in AudioEvents.Values)
            {
                audioEvent.Post(_eventReceiver);
            }
        }
    }
}
