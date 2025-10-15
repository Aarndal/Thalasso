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

        private const float MIN_RECHARGE_COOLDOWN = 0f;
        private const float MIN_RECHARGE_RATE = 1f;

        private const float MIN_RELEASE_RATE = 1f;

        [Header("Oxygen Tank Values")]
        [SerializeField, Range(MIN_TANK_VOLUME, MAX_TANK_VOLUME),
         Tooltip("The maximum amount of oxygen refilled by the refill station per use. Unit: Oxygen units.")]
        private float _maxCapacity = MAX_TANK_VOLUME;

        [SerializeField,
         Tooltip("The maximum delay until the oxygen tank starts to recharge itself. Unit: Seconds.")]
        private float _maxRechargeCooldown = 30f;
        [SerializeField, Min(MIN_RECHARGE_COOLDOWN),
         Tooltip("The minimum delay until the oxygen tank starts to recharge itself. Unit: Seconds.")]
        private float _minRechargeCooldown = 1f;
        [SerializeField, Min(MIN_RECHARGE_RATE),
         Tooltip("The rate at which the oxygen tank recharges after the delay. Unit: Oxygen units per second.")]
        private float _rechargeRate = 5f;

        [Header("Oxygen Refill Values")]
        [SerializeField,
         Tooltip("The rate at which oxygen is released by default. Unit: Oxygen units per second.")]
        private float _defaultReleaseRate = 10f;
        [SerializeField,
         Tooltip("The maximum rate at which oxygen is refilled. Unit: Oxygen units per second.")]
        private float _maxReleaseRate = 50f;
        [SerializeField, Min(MIN_RELEASE_RATE),
         Tooltip("The minimum rate at which oxygen is refilled. Unit: Oxygen units per second.")]
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
            _minRechargeCooldown = Mathf.Clamp(_minRechargeCooldown, MIN_RECHARGE_COOLDOWN, _maxRechargeCooldown);

            _rechargeRate = Mathf.Max(MIN_RECHARGE_RATE, _rechargeRate);

            _maxReleaseRate = Mathf.Max(_minReleaseRate, _maxReleaseRate);
            _minReleaseRate = Mathf.Clamp(_minReleaseRate, MIN_RELEASE_RATE, _maxReleaseRate);
            _defaultReleaseRate = Mathf.Clamp(_defaultReleaseRate, _minReleaseRate, _maxReleaseRate);
        }
    }
}