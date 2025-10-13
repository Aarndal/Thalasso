using UnityEngine;

namespace AirSupplySystem
{
    [DisallowMultipleComponent]
    public class OxygenSupply : MonoBehaviour, ILooseAir
    {
        // Serialized Fields
        [Header("References")]
        [SerializeField] private SOOxygenSupplyData _oxygenSupplyData = null;

        [Header("Settings")]
        [SerializeField] private bool _isConsumingOxygen = true;
        [SerializeField] private bool _startWithDefaultRate = false;

        // Properties
        public SOOxygenSupplyData OxygenSupplyData => _oxygenSupplyData;
        public float ActiveConsumptionRate => _oxygenSupplyData.ActiveConsumptionRate;
        public float MaxOxygenLevel => _oxygenSupplyData.MaxOxygenLevel;


        #region Unity Lifecycle Methods
        private void Awake()
        {
            if (_oxygenSupplyData == null)
            {
#if UNITY_EDITOR
                Debug.LogErrorFormat("{1} is not assigned for {0}.", gameObject.name, nameof(SOOxygenSupplyData));
#endif
                _oxygenSupplyData = ScriptableObject.CreateInstance<SOOxygenSupplyData>();
            }


            if (_startWithDefaultRate)
            {
                _oxygenSupplyData.ActiveConsumptionRate = _oxygenSupplyData.DefaultConsumptionRate;
            }

        }

        private void Reset()
        {
            ResetAirSupply();
        }

        private void OnValidate()
        {
            if (_oxygenSupplyData == null)
            {
#if UNITY_EDITOR
                Debug.LogWarningFormat("{1} is not assigned for {0}.", gameObject.name, nameof(SOOxygenSupplyData));
#endif
            }
        }

        private void OnEnable()
        {
            _oxygenSupplyData.OxygenSupplyDepleted += OnOxygenSupplyDepleted;
        }

        private void Update()
        {
            if (_isConsumingOxygen)
            {
                ConsumeOxygen();
            }
        }

        private void OnDisable()
        {
            _oxygenSupplyData.OxygenSupplyDepleted -= OnOxygenSupplyDepleted;
        }
        #endregion


        #region Callback Functions
        private void OnOxygenSupplyDepleted()
        {
            _isConsumingOxygen = false;
        }
        #endregion


        #region Public Methods
        public void ResetAirSupply()
        {
            _oxygenSupplyData.CurrentOxygenLevel = _oxygenSupplyData.MaxOxygenLevel;
            _oxygenSupplyData.ActiveConsumptionRate = 0f; // Start with no oxygen loss
        }

        public void SetAirConsumptionRate(float newAirConsumptionRate)
        {
            _oxygenSupplyData.ActiveConsumptionRate = newAirConsumptionRate;
        }

        public void SetOxygenConsumptionState(bool isConsumingOxygen = true)
        {
            if (_isConsumingOxygen == isConsumingOxygen)
                return;

            _isConsumingOxygen = isConsumingOxygen;
        }
        #endregion


        private void ConsumeOxygen()
        {
            _oxygenSupplyData.CurrentOxygenLevel -= ActiveConsumptionRate * Time.deltaTime;
        }
    }
}
