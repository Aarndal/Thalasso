using UnityEngine;
using UnityEngine.UI;

namespace AirSupplySystem
{
    public class UIAirSupplyIndicator : MonoBehaviour
    {
        [SerializeField]
        private SOOxygenSupplyData _airSupplyData;
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

            if (_airSupplyData != null)
            {
                _image.fillAmount = _airSupplyData.CurrentOxygenLevel / _airSupplyData.MaxOxygenLevel;
            }
        }

        private void OnEnable()
        {
            if (_airSupplyData != null)
            {
                _airSupplyData.AirSupplyChanged += OnAirSupplyChanged;
            }
        }

        private void OnAirSupplyChanged(float currentAirSupply)
        {
            _image.fillAmount = currentAirSupply / _airSupplyData.MaxOxygenLevel;
        }
    }
}
