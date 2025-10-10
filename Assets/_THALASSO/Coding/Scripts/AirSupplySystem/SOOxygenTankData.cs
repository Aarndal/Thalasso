using System;
using UnityEngine;

namespace AirSupplySystem
{
    /// <summary>
    /// Manages air tank data for air refill stations.
    /// </summary>
    [CreateAssetMenu(fileName = "newAirTankData", menuName = "Air Supply System/Air Tank Data", order = 1)]
    public class SOOxygenTankData : ScriptableObject
    {
        private const float MAX_TANK_VOLUME = 100f;
        private const float MIN_TANK_VOLUME = 10f;

        [Header("Air Tank Values")]
        [SerializeField, Range(MIN_TANK_VOLUME, MAX_TANK_VOLUME), Tooltip("The maximum amount of air refilled by the refill station per use. Unit: Air units.")]
        private float _maxCapacity = MAX_TANK_VOLUME;

        [SerializeField, Tooltip("The delay until the oxygen tank starts to recharge itself after having been emptied. Unit: Seconds.")]
        private float _fullRechargeCooldown = 30f;
        [SerializeField, Tooltip("The rate at which the oxygen tank recharges after the delay. Unit: Air units per second.")]
        private float _rechargeRate = 5f;

        [Header("Air Refill Values")]
        [SerializeField, Tooltip("The rate at which air is refilled by default. Unit: Air units per second.")]
        private float _defaultAirRefillRate = 10f;
        [SerializeField, Tooltip("The maximum rate at which air is refilled. Unit: Air units per second.")]
        private float _maxAirRefillRate = 50f;
        [SerializeField, Tooltip("The minimum rate at which air is refilled. Unit: Air units per second.")]
        private float _minAirRefillRate = 5f;

        public float MaxCapacity => _maxCapacity;
        public float FullRechargeCooldown => _fullRechargeCooldown;
        public float RechargeRate => _rechargeRate;
        public float DefaultAirRefillRate => _defaultAirRefillRate;
        public float MaxAirRefillRate => _maxAirRefillRate;
        public float MinAirRefillRate => _minAirRefillRate;
    }
}