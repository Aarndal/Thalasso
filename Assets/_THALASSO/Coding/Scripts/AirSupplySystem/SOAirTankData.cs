using System;
using UnityEngine;

namespace AirSupplySystem
{
    /// <summary>
    /// Manages air tank data for air refill stations.
    /// </summary>
    [CreateAssetMenu(fileName = "newAirTankData", menuName = "Air Supply System/Air Tank Data", order = 1)]
    public class SOAirTankData : ScriptableObject
    {
        private const float MAX_AIR_TANK_CAPACITY = 100f;
        private const float MIN_AIR_TANK_CAPACITY = 10f;

        [Header("Air Tank Values")]
        [SerializeField, Range(MIN_AIR_TANK_CAPACITY, MAX_AIR_TANK_CAPACITY), Tooltip("The maximum amount of air refilled per use of the refill station. Unit: Air units.")]
        private float _maxAirTankCapacity = MAX_AIR_TANK_CAPACITY;

        [SerializeField, Tooltip("The delay until the air tank can be used again after having been used. Unit: Seconds.")]
        private float _airTankRechargeDelay = 30f;
        [SerializeField, Tooltip("The rate at which the air tank recharges after the delay. Unit: Air units per second.")]
        private float _airTankRechargeRate = 5f;

        [Header("Air Refill Values")]
        [SerializeField, Tooltip("The rate at which air is refilled by default. Unit: Air units per second.")]
        private float _defaultAirRefillRate = 10f;
        [SerializeField, Tooltip("The maximum rate at which air is refilled. Unit: Air units per second.")]
        private float _maxAirRefillRate = 50f;
        [SerializeField, Tooltip("The minimum rate at which air is refilled. Unit: Air units per second.")]
        private float _minAirRefillRate = 5f;

        public float MaxAirTankCapacity => _maxAirTankCapacity;
        public float AirTankRechargeDelay => _airTankRechargeDelay;
        public float AirTankRechargeRate => _airTankRechargeRate;
        public float DefaultAirRefillRate => _defaultAirRefillRate;
    }
}