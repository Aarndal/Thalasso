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
        private const float DEFAULT_MAX_AIR_SUPPLY = 100f;

        [Header("Oxygen Supply Values")]
        
        [SerializeField, Tooltip("The maximum air supply the player can have. Unit: Air units.")]
        private float _maxOxygenLevel = DEFAULT_MAX_AIR_SUPPLY;

        [Header("Oxygen Consumption Rate Values")]
        [SerializeField, Tooltip("The default air consumption rate when no modifiers are applied. Unit: Air units per second.")]
        private float _defaultConsumptionRate = 0.05f;
        [SerializeField, Range(1f, 10f), Tooltip("The maximum air consumption rate when all modifiers are applied. Unit: Air units per second.")]
        private float _maxConsumptionRate = 5f;
        [SerializeField, Range(0f, 0.5f), Tooltip("The minimum air consumption rate when all modifiers are applied. Unit: Air units per second.")]
        private float _minConsumptionRate = 0f;


        /// <summary>
        /// Maximum air supply the player can have.
        /// </summary>
        public float MaxOxygenLevel => _maxOxygenLevel;
        /// <summary>
        /// Default air consumption rate when no modifiers are applied.
        /// </summary>
        public float DefaultConsumptionRate => _defaultConsumptionRate;
        /// <summary>
        /// Maximum air consumption rate when all modifiers are applied.
        /// </summary>
        public float MaxConsumptionRate => _maxConsumptionRate;
        /// <summary>
        /// Minimum air consumption rate when all modifiers are applied.
        /// </summary>
        public float MinConsumptionRate => _minConsumptionRate;
        
    }
}
