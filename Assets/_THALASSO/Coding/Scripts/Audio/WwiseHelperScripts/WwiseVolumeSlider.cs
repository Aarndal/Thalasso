using System;
using UnityEngine;
using UnityEngine.UI;

namespace WwiseHelper
{
    [Serializable]
    public class WwiseVolumeSlider : MonoBehaviour
    {
#if WWISE_2024_OR_LATER
        [SerializeField]
        private AK.Wwise.RTPC _rtpc = default;
        [SerializeField]
        private Slider _volumeSlider = default;

        private WwiseBus _wwiseBus;

        private void Awake()
        {
            _volumeSlider = _volumeSlider != null ? _volumeSlider : GetComponentInChildren<Slider>();

            _wwiseBus = new(_rtpc, _volumeSlider);
        }

        private void OnEnable()
        {
            _wwiseBus.LoadVolume();
            _wwiseBus.AddListener();
        }

        private void Start()
        {
            _wwiseBus.SetVolumeRange(_volumeSlider.minValue, _volumeSlider.maxValue);
        }

        private void OnDisable()
        {
            _wwiseBus.RemoveListener();
        }

        //and finally a method we can call to remove all the user data!
        public void DeleteUserData()
        {
            _wwiseBus.DeleteData();
            _wwiseBus.LoadVolume();
        }
#endif
    }
}
