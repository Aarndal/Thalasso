using System.Collections.Generic;
using UnityEngine;

namespace WwiseHelper
{
    public abstract class WwiseEventHandler : MonoBehaviour
    {
#if WWISE_2024_OR_LATER
        [Header("References")]
        [SerializeField]
        protected AkGameObj _akGameObject = default;

        [Header("Wwise Events")]
        [SerializeField]
        protected bool areEnvironmentAware = false;
        [SerializeField]
        protected AK.Wwise.Event[] _wwiseEvents;

        protected AkRoomAwareObject _akRoomAwareObject = default;
        protected Rigidbody _rigidbody = default;

        public readonly Dictionary<string, AK.Wwise.Event> AudioEvents = new();
#endif

        protected virtual void Awake()
        {
#if WWISE_2024_OR_LATER
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

            if (areEnvironmentAware)
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
#endif
        }
    }
}
