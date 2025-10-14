using System;
using UnityEngine;

namespace OxygenSupplySystem
{
    //!TODO: Separate the consumption logic into its own component to allow for more modularity.
    //?Question: Update Logic: Move to FixedUpdate()?

    [DisallowMultipleComponent]
    public class OxygenSupply : MonoBehaviour, IConsumeOxygen
    {
        #region Serialized Fields

        [Header("References")]
        [SerializeField]
        private SOOxygenSupplyData _oxygenSupplyData = null;

        [Header("Settings")]
        [SerializeField]
        private bool _isConsumingOxygen = true;
        [SerializeField]
        private bool _startWithNoConsumption = true;

        #endregion


        #region Private Members

        private float _currentOxygenLevel = 100f;
        private float _currentMaxOxygenLevel = 0f;
        private float _activeConsumptionRate = 0f;

        #endregion


        #region Properties

        /// <summary>
        /// Rate at which air is consumed.
        /// Unit: Air units per second.
        /// Clamped between MinAirConsumptionRate and MaxAirConsumptionRate.
        /// </summary>
        public float OxygenConsumptionRate
        {
            get => _activeConsumptionRate;
            set => _activeConsumptionRate = Mathf.Clamp(value, _oxygenSupplyData.MinConsumptionRate, _oxygenSupplyData.MaxConsumptionRate);
        }

        /// <summary>
        /// Current air supply the player has.
        /// Clamped between default MinAirSupply and MaxAirSupply.
        /// </summary>
        public float OxygenLevel
        {
            get => _currentOxygenLevel;
            set
            {
                if (_currentOxygenLevel != value)
                {
                    _currentOxygenLevel = Mathf.Clamp(value, 0f, _currentMaxOxygenLevel);
                    OxygenSupplyChanged?.Invoke(_currentOxygenLevel);

                    if (_currentOxygenLevel >= _currentMaxOxygenLevel)
                    {
                        _currentOxygenLevel = _currentMaxOxygenLevel;
                        OxygenSupplyReachedMax?.Invoke();
                    }

                    if (_currentOxygenLevel <= 0.0f)
                    {
                        _currentOxygenLevel = 0.0f;
#if UNITY_EDITOR
                        Debug.LogWarning("Oxygen supply has been depleted!");
#endif
                        OxygenSupplyDepleted?.Invoke();

                        _isConsumingOxygen = false;
                    }
                }
            }
        }

        public float MaxOxygenLevel => _currentMaxOxygenLevel;

        #endregion


        #region Events

        public event Action OxygenSupplyDepleted;
        public event Action OxygenSupplyReachedMax;
        public event Action<float> OxygenSupplyChanged;
        public event Action<float> OxygenConsumptionRateChanged;
        public event Action<float> MaxOxygenSupplyChanged;

        #endregion


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



        }

        private void Reset()
        {
            ResetOxygenSupply();
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

        private void Start()
        {
            _isConsumingOxygen = !_startWithNoConsumption;

            _currentMaxOxygenLevel = _oxygenSupplyData.MaxOxygenLevel;
            OxygenLevel = _currentMaxOxygenLevel;

            OxygenConsumptionRate = _oxygenSupplyData.DefaultConsumptionRate;
        }

        private void Update()
        {
            //?Question: Move to FixedUpdate()?
            ConsumeOxygen();
        }

        #endregion


        #region Public Methods
        
        public bool ConsumeOxygen()
        {
            if (!_isConsumingOxygen)
            {
                return false;
            }
            
            OxygenLevel -= OxygenConsumptionRate * Time.deltaTime;
            return true;
        }

        public void ResetOxygenSupply()
        {
            OxygenLevel = _oxygenSupplyData.MaxOxygenLevel;
            OxygenConsumptionRate = 0f; // Start with no oxygen loss
        }

        public void SetOxygenConsumptionState(bool isConsumingOxygen = true)
        {
            if (_isConsumingOxygen == isConsumingOxygen)
                return;

            _isConsumingOxygen = isConsumingOxygen;
        }

        #endregion
    }
}
