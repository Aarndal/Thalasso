using UnityEngine;
using UnityEngine.UI;

namespace WwiseHelper
{
    public class WwiseBus
    {
#if WWISE_2024_OR_LATER
        private float _volume = 0.8f;
        private float _volumeRange = 1.0f;


        private readonly AK.Wwise.RTPC _rtpc;
        private readonly Slider _volumeSlider;


        public WwiseBus(AK.Wwise.RTPC rtpc, Slider volumeSlider)
        {
            _rtpc = rtpc;
            _volumeSlider = volumeSlider;

            Volume = 0.8f; // set Volume at 80%
        }

        public float Volume
        {
            get => _volume;
            private set
            {
                if (value != _volume)
                {
                    value /= _volumeRange;

                    if (value >= 1.0f)
                        value = 1.0f;

                    if (value <= 0.0f)
                        value = 0.0f;

                    _volume = value;
                }
            }
        }

        public void AddListener()
        {
            _volumeSlider.onValueChanged.AddListener((value) => SetVolume(value));
        }

        public void DeleteData()
        {
            PlayerPrefs.DeleteKey(_rtpc.Name);
        }

        public void LoadVolume()
        {
            if (!PlayerPrefs.HasKey(_rtpc.Name))
                PlayerPrefs.SetFloat(_rtpc.Name, Volume);

            _volumeSlider.value = PlayerPrefs.GetFloat(_rtpc.Name);
        }

        public void RemoveListener()
        {
            _volumeSlider.onValueChanged.RemoveListener((value) => SetVolume(value));
        }

        public void SetVolumeRange(float minValue, float maxValue)
        {
            _volumeRange = maxValue - minValue;
        }

        private void SaveVolume()
        {
            PlayerPrefs.SetFloat(_rtpc.Name, Volume);
        }

        private void SetVolume(float volume)
        {
            Volume = volume;
            AkUnitySoundEngine.SetRTPCValue(_rtpc.Id, Volume);
            SaveVolume();
        }
#endif
    }
}
