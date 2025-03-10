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
        protected bool _areEnvironmentAware = false;
        
        [Header("References")]
        [SerializeField]
        protected AkGameObj _akGameObject = default;

        protected AkRoomAwareObject _akRoomAwareObject = default;
        protected Rigidbody _rigidbody = default;

        public readonly Dictionary<string, AK.Wwise.Event> AudioEvents = new();

        protected virtual void Awake()
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
#endif
    }
}
