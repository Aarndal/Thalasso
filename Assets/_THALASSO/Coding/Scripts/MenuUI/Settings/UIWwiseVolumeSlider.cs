using System;
using UnityEngine;
using UnityEngine.UI;

namespace WwiseHelper
{
    [Serializable]
    public class UIWwiseVolumeSlider : MonoBehaviour
    {
#if WWISE_2024_OR_LATER
        [SerializeField]
        private AK.Wwise.RTPC _rtpc = default;
        [SerializeField]
        private Slider _volumeSlider = default;

        private WwiseBus _wwiseBus;

        public WwiseBus AudioBus => _wwiseBus;

        private void Awake()
        {
            _volumeSlider = _volumeSlider != null ? _volumeSlider : GetComponentInChildren<Slider>();

            if(_volumeSlider == null)
            {
                Debug.LogErrorFormat("<color=cyan>{0}</color> <color=red>has no Slider attached!</color> {1} needs a Slider component on its associated GameObject or one of its children to function.", gameObject.name, this);
            }

            if(!_rtpc.IsValid())
            {
                _rtpc.Validate();
                return;
            }

            _wwiseBus ??= new(_rtpc, _volumeSlider);
        }

        private void OnEnable()
        {
            _wwiseBus.LoadData();
        }

        private void Start()
        {
            _wwiseBus.AddListener();
        }

        private void OnDestroy()
        {
            _wwiseBus.RemoveListener();
        }

        public void DeleteUserData()
        {
            _wwiseBus.DeleteData();
            _wwiseBus.LoadData();
        }
#endif
    }
}
