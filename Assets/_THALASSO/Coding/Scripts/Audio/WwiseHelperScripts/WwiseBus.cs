using UnityEngine;
using UnityEngine.UI;

namespace WwiseHelper
{
    public class WwiseBus
    {
#if WWISE_2024_OR_LATER
        private float _volume = 0.8f;

        private readonly AK.Wwise.RTPC _rtpc;
        private readonly Slider _volumeSlider;

        public WwiseBus(AK.Wwise.RTPC rtpc, Slider slider)
        {
            _rtpc = rtpc;
            _volumeSlider = slider;

            Volume = 0.8f; // set Volume at 80%
        }

        public string RTPCName => _rtpc.Name;

        public float Volume
        {
            get => _volume;
            private set
            {
                if (value >= 1.0f)
                    value = 1.0f;

                if (value <= 0.0f)
                    value = 0.0f;

                _volume = value;

                AkUnitySoundEngine.SetRTPCValue(_rtpc.Id, _volume);
            }
        }

        #region Callback Registration/Deregistration
        public void AddListener()
        {
            GlobalEventBus.Register(GlobalEvents.UI.MenuClosed, OnMenuClosed);
            _volumeSlider.onValueChanged.AddListener(SetVolume);
        }


        public void RemoveListener()
        {
            _volumeSlider.onValueChanged.RemoveListener(SetVolume);
            GlobalEventBus.Deregister(GlobalEvents.UI.MenuClosed, OnMenuClosed);
        }
        #endregion

        #region Public Methods
        public void LoadData()
        {
            if (!PlayerPrefs.HasKey(_rtpc.Name))
                PlayerPrefs.SetFloat(_rtpc.Name, Volume);

            Volume = PlayerPrefs.GetFloat(_rtpc.Name);

            _volumeSlider.value = _volumeSlider.maxValue * Volume + _volumeSlider.minValue * (1 - Volume);
        }

        public void DeleteData()
        {
            if (PlayerPrefs.HasKey(_rtpc.Name))
                PlayerPrefs.DeleteKey(_rtpc.Name);
        }
        #endregion

        #region Private Methods
        private void OnMenuClosed(object[] eventArgs)
        {
            SaveData();
        }

        private void SaveData()
        {
            PlayerPrefs.SetFloat(_rtpc.Name, Volume);
        }

        private void SetVolume(float volume)
        {
            if (_volumeSlider.normalizedValue != Volume)
                Volume = _volumeSlider.normalizedValue;
        }
        #endregion
#endif
    }
}
