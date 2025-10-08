using System;
using UnityEngine;
using UnityEngine.UI;

namespace AirSupplySystem
{
    [RequireComponent(typeof(Image))]
    public class UIFillAmountIndicator : MonoBehaviour
    {
        [SerializeField]
        private AirRefillStation _airRefillStation = null;

        [SerializeField]
        private Gradient _colorGradient = null;

        private Image _image = null;

        private void Awake()
        {
            if (!TryGetComponent(out _image))
            {
#if UNITY_EDITOR
                Debug.LogErrorFormat("No Image component found on {0}. Please assign an Image component in the inspector.", gameObject.name);
#endif
            }

            if (_airRefillStation == null)
            {
#if UNITY_EDITOR
                Debug.LogErrorFormat("No {1} assigned on {0}. Please assign an {1} in the inspector.", gameObject.name, typeof(AirRefillStation).Name);
#endif
            }
        }

        private void OnEnable()
        {
            if (_airRefillStation != null)
            {
                _airRefillStation.AirTankCapacityChanged += OnAirTankCapacityChanged;
            }
        }

        private void Start()
        {
            _image.fillAmount = _airRefillStation.CurrentAirTankCapacity / _airRefillStation.MaxAirTankCapacity;
            _image.color = _colorGradient.Evaluate(1 - _image.fillAmount);
        }

        void OnAirTankCapacityChanged(float newFillAmount)
        {
            _image.fillAmount = newFillAmount / _airRefillStation.MaxAirTankCapacity;
            _image.color = _colorGradient.Evaluate(1 - _image.fillAmount);
        }
    }
}
