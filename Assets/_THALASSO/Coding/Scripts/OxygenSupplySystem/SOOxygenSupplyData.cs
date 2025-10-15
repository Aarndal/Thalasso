using System;
using UnityEngine;

namespace OxygenSupplySystem
{
    /// <summary>
    /// Manages oxygen supply data for the player character.
    /// </summary>
    [CreateAssetMenu(fileName = "newOxygenSupplyData", menuName = "Oxygen Supply System/Oxygen Supply Data", order = 0)]
    public class SOOxygenSupplyData : ScriptableObject
    {
        private const float DEFAULT_MAX_OXYGEN_SUPPLY = 100f;

        [Header("Oxygen Supply Values")]
        [SerializeField, Min(50f), // Allow at least 50 units of oxygen
         Tooltip("The maximum oxygen supply the player can have. Unit: Oxygen units.")]
        private float _maxOxygenLevel = DEFAULT_MAX_OXYGEN_SUPPLY;

        [Header("Oxygen Consumption Rate Values")]
        [SerializeField, 
         Tooltip("The default oxygen consumption rate when no modifiers are applied. Unit: Oxygen units per second.")]
        private float _defaultConsumptionRate = 0.05f;
        [SerializeField, 
         Tooltip("The maximum oxygen consumption rate when all modifiers are applied. Unit: Oxygen units per second.")]
        private float _maxConsumptionRate = 5f;
        [SerializeField, Min(0f), 
         Tooltip("The minimum oxygen consumption rate when all modifiers are applied. Unit: Oxygen units per second.")]
        private float _minConsumptionRate = 0f;


        /// <summary>
        /// Maximum oxygen supply the player can have. Unit: Oxygen units.
        /// </summary>
        public float MaxOxygenLevel => _maxOxygenLevel;
        /// <summary>
        /// Default oxygen consumption rate when no modifiers are applied. Unit: Oxygen units per second.
        /// </summary>
        public float DefaultConsumptionRate => _defaultConsumptionRate;
        /// <summary>
        /// Maximum oxygen consumption rate when all modifiers are applied. Unit: Oxygen units per second.
        /// </summary>
        public float MaxConsumptionRate => _maxConsumptionRate;
        /// <summary>
        /// Minimum oxygen consumption rate when all modifiers are applied. Unit: Oxygen units per second.
        /// </summary>
        public float MinConsumptionRate => _minConsumptionRate;


        private void OnValidate()
        {
            _maxConsumptionRate = Mathf.Max(_minConsumptionRate, _maxConsumptionRate);
            _minConsumptionRate = Mathf.Clamp(_minConsumptionRate, 0f, _maxConsumptionRate);
            _defaultConsumptionRate = Mathf.Clamp(_defaultConsumptionRate, _minConsumptionRate, _maxConsumptionRate);
        }
    }
}
