using UnityEngine;
#if WWISE_2024_OR_LATER
using static AkEnvironment;
#endif

namespace WwiseHelper
{
    [RequireComponent(typeof(Collider))]
    public sealed class WwiseEnvironmentEventHandler : MonoBehaviour
    {
#if WWISE_2024_OR_LATER
        [SerializeField]
        private AK.Wwise.AuxBus _wwiseAuxBus = default;
        [SerializeField]
        private AkRoom _room = default;
        [SerializeField]
        private AkAuxSendValue _;

        private Collider _collider = default;
        AkEnvironment_CompareBySelectionAlgorithm _comparer = new();


        private void Awake()
        {
            _collider = _collider != null ? _collider : GetComponent<Collider>();
        }

        private void Start()
        {
            _collider.isTrigger = true;
            _room.reverbAuxBus = _wwiseAuxBus;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out AkRoomAwareObject target))
            {
                target.EnteredRoom(_room);

                //AkUnitySoundEngine.SetGameObjectAuxSendValues(target.gameObject, AkAuxSendValue , 1);
            }
        }
#endif
    }
}
