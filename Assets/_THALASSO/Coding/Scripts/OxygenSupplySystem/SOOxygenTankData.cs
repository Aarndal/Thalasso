using System;
using UnityEngine;

namespace OxygenSupplySystem
{
    /// <summary>
    /// Manages oxygen tank data for oxygen refill stations.
    /// </summary>
    [CreateAssetMenu(fileName = "newOxygenTankData", menuName = "Oxygen Supply System/Oxygen Tank Data", order = 1)]
    public class SOOxygenTankData : ScriptableObject
    {
        private const float MAX_TANK_VOLUME = 100f;
        private const float MIN_TANK_VOLUME = 10f;

        [Header("Oxygen Tank Values")]
        [SerializeField, Range(MIN_TANK_VOLUME, MAX_TANK_VOLUME), Tooltip("The maximum amount of air refilled by the refill station per use. Unit: Air units.")]
        private float _maxCapacity = MAX_TANK_VOLUME;

        [SerializeField, Tooltip("The maximum delay until the oxygen tank starts to recharge itself. Unit: Seconds.")]
        private float _maxRechargeCooldown = 30f;
        [SerializeField, Min(0f), Tooltip("The minimum delay until the oxygen tank starts to recharge itself. Unit: Seconds.")]
        private float _minRechargeCooldown = 1f;
        [SerializeField, Min(1f), Tooltip("The rate at which the oxygen tank recharges after the delay. Unit: Air units per second.")]
        private float _rechargeRate = 5f;

        [Header("Oxygen Refill Values")]
        [SerializeField, Tooltip("The rate at which air is released by default. Unit: Air units per second.")]
        private float _defaultReleaseRate = 10f;
        [SerializeField, Tooltip("The maximum rate at which air is refilled. Unit: Air units per second.")]
        private float _maxReleaseRate = 50f;
        [SerializeField, Range(1f, 30f), Tooltip("The minimum rate at which air is refilled. Unit: Air units per second.")]
        private float _minReleaseRate = 5f;

        public float MaxCapacity => _maxCapacity;
        public float MaxRechargeCooldown => _maxRechargeCooldown;
        public float MinRechargeCooldown => _minRechargeCooldown;
        public float RechargeRate => _rechargeRate;
        public float DefaultReleaseRate => _defaultReleaseRate;
        public float MaxReleaseRate => _maxReleaseRate;
        public float MinReleaseRate => _minReleaseRate;


        private void OnValidate()
        {
            _maxCapacity = Mathf.Clamp(_maxCapacity, MIN_TANK_VOLUME, MAX_TANK_VOLUME);
            
            _maxRechargeCooldown = Mathf.Max(_minRechargeCooldown, _maxRechargeCooldown);
            _minRechargeCooldown = Mathf.Clamp(_minRechargeCooldown, 0f, _maxRechargeCooldown);
            
            _rechargeRate = Mathf.Max(1f, _rechargeRate);
            
            _maxReleaseRate = Mathf.Max(_minReleaseRate, _maxReleaseRate);
            _minReleaseRate = Mathf.Clamp(_minReleaseRate, 0f, _maxReleaseRate);
            _defaultReleaseRate = Mathf.Clamp(_defaultReleaseRate, _minReleaseRate, _maxReleaseRate);
        }
    }
}