using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WwiseHelper
{
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

        protected AkGameObj _akGameObject = default;

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

            if (_playOnOtherObject && _eventReceiver != triggeringObject)
            {
                SetEventReceiver(triggeringObject);
                InitRequiredWwiseComponents();
            }

            if (responderState == ResponderState.Switch)
            {
                responderState = CurrentState == ResponderState.Off ? ResponderState.On : ResponderState.Off;
            }

            switch (responderState)
            {
                case ResponderState.None:
                    Debug.LogWarningFormat("<color=yellow>No valid ResponderState set</color> for Trigger activated by <color=cyan>{0}</color>", triggeringObject);
                    return;
                case ResponderState.Off:
                    TurnOff();
                    break;
                case ResponderState.On:
                    TurnOn();
                    break;
                default:
                    TurnOn();
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

        protected void TurnOff()
        {
#if WWISE_2024_OR_LATER
            CurrentState = ResponderState.Off;

            foreach (var audioEvent in AudioEvents.Values)
            {
                audioEvent.Stop(_akGameObject);
            }
#endif
        }

        protected void TurnOn()
        {
#if WWISE_2024_OR_LATER
            CurrentState = ResponderState.On;

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
