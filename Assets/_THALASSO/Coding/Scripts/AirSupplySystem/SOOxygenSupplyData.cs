using System;
using UnityEngine;

namespace AirSupplySystem
{
    /// <summary>
    /// Manages air supply data for the player character.
    /// </summary>
    [CreateAssetMenu(fileName = "newAirSupplyData", menuName = "Air Supply System/Air Supply Data", order = 0)]
    public class SOOxygenSupplyData : ScriptableObject
    {
        private const float DEFAULT_MAX_AIR_SUPPLY = 100f;

        [Header("Air Supply Values")]
        [SerializeField, Tooltip("For Debugging purposes only. Unit: Air units.")]
        private float _currentAirSupply = 100f;
        [SerializeField, Tooltip("The maximum air supply the player can have. Unit: Air units.")]
        private float _maxAirSupply = DEFAULT_MAX_AIR_SUPPLY;

        [Header("Air Consumption Rate Values")]
        [SerializeField, Tooltip("For Debugging purposes only. Unit: Air units per second.")]
        private float _currentAirConsumptionRate = 0f;
        [SerializeField, Tooltip("The default air consumption rate when no modifiers are applied. Unit: Air units per second.")]
        private float _defaultAirConsumptionRate = 0.05f;
        [SerializeField, Range(1f, 10f), Tooltip("The maximum air consumption rate when all modifiers are applied. Unit: Air units per second.")]
        private float _maxAirConsumptionRate = 5f;
        [SerializeField, Range(0f, 0.5f), Tooltip("The minimum air consumption rate when all modifiers are applied. Unit: Air units per second.")]
        private float _minAirConsumptionRate = 0f;


        /// <summary>
        /// Maximum air supply the player can have.
        /// </summary>
        public float MaxOxygenLevel => _maxAirSupply;
        /// <summary>
        /// Default air consumption rate when no modifiers are applied.
        /// </summary>
        public float DefaultConsumptionRate => _defaultAirConsumptionRate;
        /// <summary>
        /// Maximum air consumption rate when all modifiers are applied.
        /// </summary>
        public float MaxAirConsumptionRate => _maxAirConsumptionRate;
        /// <summary>
        /// Minimum air consumption rate when all modifiers are applied.
        /// </summary>
        public float MinAirConsumptionRate => _minAirConsumptionRate;


        public event Action OxygenSupplyDepleted;
        public event Action AirSupplyReachedMax;
        public event Action<float> AirSupplyChanged;
        public event Action<float> AirConsumptionRateChanged;
        public event Action<float> MaxAirSupplyChanged;


        /// <summary>
        /// Current air supply the player has.
        /// Clamped between default MinAirSupply and MaxAirSupply.
        /// </summary>
        public float CurrentOxygenLevel
        {
            get => _currentAirSupply;
            set
            {
                if (_currentAirSupply != value)
                {
                    _currentAirSupply = Mathf.Clamp(value, 0f, _maxAirSupply);
                    AirSupplyChanged?.Invoke(_currentAirSupply);

                    if (_currentAirSupply >= _maxAirSupply)
                    {
                        _currentAirSupply = _maxAirSupply;
                        AirSupplyReachedMax?.Invoke();
                    }

                    if (_currentAirSupply <= 0.0f)
                    {
                        _currentAirSupply = 0.0f;
#if UNITY_EDITOR
                        Debug.LogWarning("Air supply has been depleted!");
#endif
                        OxygenSupplyDepleted?.Invoke();
                    }
                }
            }
        }


        /// <summary>
        /// Rate at which air is consumed.
        /// Unit: Air units per second.
        /// Clamped between MinAirConsumptionRate and MaxAirConsumptionRate.
        /// </summary>
        public float ActiveConsumptionRate
        {
            get => _currentAirConsumptionRate;
            set => _currentAirConsumptionRate = Mathf.Clamp(value, MinAirConsumptionRate, MaxAirConsumptionRate);
        }
    }
}
