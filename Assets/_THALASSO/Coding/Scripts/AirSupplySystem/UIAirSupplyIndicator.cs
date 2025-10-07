using UnityEngine;
using UnityEngine.UI;

namespace AirSupplySystem
{
    [RequireComponent(typeof(Image))]
    public class UIAirSupplyIndicator : MonoBehaviour
    {
        [SerializeField]
        private SOAirSupplyData _airSupplyData;
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
                _image.fillAmount = _airSupplyData.CurrentAirSupply / _airSupplyData.MaxAirSupply;
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
            _image.fillAmount = currentAirSupply / _airSupplyData.MaxAirSupply;
        }
    }
}
