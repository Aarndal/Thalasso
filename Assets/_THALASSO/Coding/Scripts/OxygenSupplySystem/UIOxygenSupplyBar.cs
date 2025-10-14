using UnityEngine;
using UnityEngine.UI;

namespace OxygenSupplySystem
{
    public class UIOxygenSupplyBar : MonoBehaviour
    {
        [SerializeField]
        private OxygenSupply _oxygenSupply;
        [SerializeField]
        private Image _image;

        private void Awake()
        {
            if (_image == null)
            {
                if (!TryGetComponent<Image>(out var img))
                {
#if UNITY_EDITOR
                    Debug.LogErrorFormat("No Image component found on {0}. Please assign an Image component in the inspector.", gameObject.name);
#endif
                }
                _image = img;
            }

            if (_oxygenSupply != null)
            {
                _image.fillAmount = _oxygenSupply.OxygenLevel / _oxygenSupply.MaxOxygenLevel;
            }
        }

        private void OnEnable()
        {
            if (_oxygenSupply != null)
            {
                _oxygenSupply.OxygenSupplyChanged += OnAirSupplyChanged;
            }
        }

        private void OnDisable()
        {
            if (_oxygenSupply != null)
            {
                _oxygenSupply.OxygenSupplyChanged -= OnAirSupplyChanged;
            }
        }

        private void OnAirSupplyChanged(float currentAirSupply)
        {
            _image.fillAmount = currentAirSupply / _oxygenSupply.MaxOxygenLevel;
        }
    }
}
