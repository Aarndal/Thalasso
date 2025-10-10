using UnityEngine;
using UnityEngine.UI;

namespace AirSupplySystem
{
    [RequireComponent(typeof(Image))]
    public class UIFillAmountIndicator : MonoBehaviour
    {
        [SerializeField]
        private OxygenRefillStationResponder _airRefillStation = null;

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
                Debug.LogErrorFormat("No {1} assigned on {0}. Please assign an {1} in the inspector.", gameObject.name, typeof(OxygenRefillStationResponder).Name);
#endif
            }
        }

        private void OnEnable()
        {
            if (_airRefillStation != null)
            {
                _airRefillStation.OxygenTank.CapacityChanged += OnOxygenTankCapacityChanged;
            }
        }

        private void Start()
        {
            _image.fillAmount = _airRefillStation.OxygenTank.FillingDegree;
            _image.color = _colorGradient.Evaluate(1 - _image.fillAmount);
        }

        private void OnDisable()
        {
            if (_airRefillStation != null)
            {
                _airRefillStation.OxygenTank.CapacityChanged -= OnOxygenTankCapacityChanged;
            }
        }

        void OnOxygenTankCapacityChanged(float newFillAmount, float fillingDegree)
        {
            _image.fillAmount = fillingDegree;
            _image.color = _colorGradient.Evaluate(1 - _image.fillAmount);
        }
    }
}
