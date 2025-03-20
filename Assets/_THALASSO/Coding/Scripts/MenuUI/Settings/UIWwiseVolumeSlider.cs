using UnityEngine;
using UnityEngine.UI;

namespace WwiseHelper
{
    [RequireComponent(typeof(Slider))]
    public class UIWwiseVolumeSlider : SettingElement<float>
    {
#if WWISE_2024_OR_LATER
        [SerializeField]
        private AK.Wwise.RTPC _rtpc = default;

        private Slider _volumeSlider = default;
        private float _volume = 0.8f;

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

                AkUnitySoundEngine.SetRTPCValue(_rtpc.Id, _volume);

                if (value != _volume)
                {
                    _volume = value;
                    _valueChanged?.Invoke(ID, _volume);
                }
            }
        }

        private void Awake()
        {
            if (!gameObject.TryGetComponent(out _volumeSlider))
                _volumeSlider = gameObject.AddComponent<Slider>();

            if (!_rtpc.IsValid())
            {
                _rtpc.Validate();
                return;
            }
        }


        #region Data Management Methods
        public override void LoadData()
        {
            if (!PlayerPrefs.HasKey(_rtpc.Name))
            {
                PlayerPrefs.SetFloat(_rtpc.Name, Volume);
            }

            Volume = PlayerPrefs.GetFloat(_rtpc.Name);

            _volumeSlider.value = _volumeSlider.maxValue * Volume + _volumeSlider.minValue * (1 - Volume);
        }

        protected override void SetData(float data)
        {
            if (_volumeSlider.normalizedValue != Volume)
                Volume = _volumeSlider.normalizedValue;
        }

        public override void SaveData()
        {
            PlayerPrefs.SetFloat(_rtpc.Name, Volume);
        }

        public override void DeleteData()
        {
            if (PlayerPrefs.HasKey(_rtpc.Name))
                PlayerPrefs.DeleteKey(_rtpc.Name);

            LoadData();
        }
        #endregion


        #region Callback Functions
        protected override void AddListener()
        {
            base.AddListener();
            _volumeSlider.onValueChanged.AddListener(SetData);
        }

        protected override void RemoveListener()
        {
            _volumeSlider.onValueChanged.RemoveListener(SetData);
            base.RemoveListener();
        }
        #endregion
#endif
    }
}
