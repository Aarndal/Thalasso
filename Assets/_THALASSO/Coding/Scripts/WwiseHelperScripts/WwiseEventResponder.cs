using System.Collections.Generic;
using UnityEngine;

namespace WwiseHelper
{
#if WWISE_2024_OR_LATER
    [RequireComponent(typeof(AkGameObj))]
#endif
    public class WwiseEventResponder : ResponderBase
    {
#if WWISE_2024_OR_LATER
        [Header("Wwise Event Settings")]
        [SerializeField]
        private bool _areOneTimeEvents = false;
        [SerializeField]
        protected bool _areEnvironmentAware = false;
        [SerializeField]
        protected bool _playOnOtherObject = false; // ToDo: Implement so that a WwiseEvent can be posted to the triggeringObject.
        [SerializeField]
        protected TriggerState _startState = TriggerState.TurnOff;

        [Header("Wwise Events")]
        [SerializeField]
        protected AK.Wwise.Event[] _wwiseEvents;

        protected TriggerState _currentTriggerState = TriggerState.None;
        
        protected AkGameObj _akGameObject = default;
        protected AkRoomAwareObject _akRoomAwareObject = default;
        protected Rigidbody _rigidbody = default;

        public readonly Dictionary<string, AK.Wwise.Event> AudioEvents = new();

        protected override void Awake()
        {
            base.Awake();

            _currentTriggerState = _startState;

            if (_wwiseEvents != null || _wwiseEvents.Length == 0)
            {
                foreach (var wwiseEvent in _wwiseEvents)
                {
                    if (!AudioEvents.TryAdd(wwiseEvent.Name, wwiseEvent))
                    {
                        Debug.LogErrorFormat("Wwise Event color=cyan>{0}</color> <color=red>is defined multiple times</color> in <color=cyan>{1}</color> WwiseEventAnimationEventResponder component!", wwiseEvent.Name, gameObject.name);
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

        public override void Respond(GameObject triggeringObject, TriggerState triggerState)
        {
            if (triggerState == TriggerState.None)
            {
                Debug.LogFormat("No valid TriggerState set for Trigger activated by {0}", triggeringObject);
                return;
            }

            if (triggerState == TriggerState.Switch)
            {
                triggerState = _currentTriggerState == TriggerState.TurnOff ? TriggerState.TurnOn : TriggerState.TurnOff;
            }

            if (triggerState == TriggerState.TurnOn)
            {
                _currentTriggerState = TriggerState.TurnOn;
                TriggerIsTurnedOn();
            }

            if (triggerState == TriggerState.TurnOff)
            {
                _currentTriggerState = TriggerState.TurnOff;
                TriggerIsTurnedOff();
            }

            if (_areOneTimeEvents)
            {
                foreach (var triggerable in _triggers)
                {
                    if (triggerable.Interface.IsTriggerable)
                        triggerable.Interface.ChangeIsTriggerable();
                }
            }
#endif
        }

        protected void TriggerIsTurnedOff()
        {
            foreach (var audioEvent in AudioEvents.Values)
            {
                audioEvent.Stop(_akGameObject.gameObject);
            }
            //AkUnitySoundEngine.StopAll(_akGameObject.gameObject);
        }

        protected void TriggerIsTurnedOn()
        {
            foreach (var audioEvent in AudioEvents.Values)
            {
                audioEvent.Post(_akGameObject.gameObject);
            }
        }
    }
}
