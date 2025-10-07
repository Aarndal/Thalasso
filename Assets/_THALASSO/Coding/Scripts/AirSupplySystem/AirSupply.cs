using UnityEngine;

namespace AirSupplySystem
{
    [DisallowMultipleComponent]
    public class AirSupply : MonoBehaviour, ILooseAir
    {
        // Serialized Fields
        [Header("References")]
        [SerializeField] private SOAirSupplyData _airSupplyData = null;

        [Header("Settings")]
        [SerializeField] private bool _isConsumingAir = true;
        [SerializeField] private bool _startWithDefaultRate = false;

        // Properties
        public SOAirSupplyData AirSupplyData => _airSupplyData;
        public float CurrentAirConsumptionRate => _airSupplyData.CurrentAirConsumptionRate;
        public float MaxAirSupply => _airSupplyData.MaxAirSupply;


        #region Unity Lifecycle Methods
        private void Awake()
        {
            if (_airSupplyData == null)
            {
#if UNITY_EDITOR
                Debug.LogErrorFormat("AirSupplyData is not assigned for {0}", gameObject.name);
#endif
            }

            ResetAirSupply();

            if (_startWithDefaultRate)
            {
                _airSupplyData.CurrentAirConsumptionRate = _airSupplyData.DefaultAirConsumptionRate;
            }
        }

        private void OnEnable()
        {
            _airSupplyData.AirSupplyDepleted += OnAirSupplyDepleted;
        }

        private void Update()
        {
            if (_isConsumingAir)
            {
                DepleteAirSupply();
            }
        }

        private void OnDisable()
        {
            _airSupplyData.AirSupplyDepleted -= OnAirSupplyDepleted;
        }
        #endregion


        #region Callback Functions
        private void OnAirSupplyDepleted()
        {
            _isConsumingAir = false;
        }
        #endregion


        #region Public Methods
        public void ResetAirSupply()
        {
            _airSupplyData.CurrentAirSupply = _airSupplyData.MaxAirSupply;
            _airSupplyData.CurrentAirConsumptionRate = 0f; // Start with no air loss
        }

        public void SetAirConsumptionRate(float newAirConsumptionRate)
        {
            _airSupplyData.CurrentAirConsumptionRate = newAirConsumptionRate;
        }

        public void SetAirConsumptionState(bool isConsumingAir = true)
        {
            _isConsumingAir = isConsumingAir;
        }
        #endregion


        private void DepleteAirSupply()
        {
            _airSupplyData.CurrentAirSupply -= CurrentAirConsumptionRate * Time.deltaTime;
        }
    }
}
