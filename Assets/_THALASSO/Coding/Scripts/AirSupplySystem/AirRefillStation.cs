using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace AirSupplySystem
{
    [DisallowMultipleComponent]
    public class AirRefillStation : InteractiveTrigger
    {
        // Serialized Fields
        [Header("References")]
        [SerializeField]
        private SOAirTankData _airTankData = null;

        [Header("Settings")]
        [SerializeField, Tooltip("Determines if the air refill station can be interacted with.")]
        bool _isActive = true;
        [SerializeField, Min(0.0f), Tooltip("For debugging purposes only. Can't be below 0.")]
        private float _currentAirTankCapacity = 0.0f;

        // Private Members
        private bool _isInUse = false;
        private bool _isRecharging = false;
        private CancellationTokenSource _cancellationTokenSource = new();

        // Properties
        public float CurrentAirTankCapacity
        {
            get => _currentAirTankCapacity;
            set
            {
                if (value == _currentAirTankCapacity)
                    return;

                _currentAirTankCapacity = Mathf.Clamp(value, 0.0f, MaxAirTankCapacity);

                AirTankCapacityChanged?.Invoke(_currentAirTankCapacity);

                if (_currentAirTankCapacity <= 0.0f)
                {
                    AirTankEmptied?.Invoke();
                    return;
                }

                if (_currentAirTankCapacity >= MaxAirTankCapacity)
                {
                    AirTankFull?.Invoke();
                    return;
                }
            }
        }
        public override bool IsActivatable => _isActive && !_isRecharging && _isTriggerable;
        public float MaxAirTankCapacity => _airTankData != null ? _airTankData.MaxAirTankCapacity : 0.0f;

        // Events
        public event Action AirRefillingStopped;
        public event Action<float> AirTankCapacityChanged;
        public event Action AirTankEmptied;
        public event Action AirTankFull;
        public event Action AirTankRecharging;


        #region Unity Lifecycle Methods
        protected override void Awake()
        {
            base.Awake();

            if (_airTankData == null)
            {
#if UNITY_EDITOR
                Debug.LogErrorFormat("{1} is not assigned for {0}", gameObject.name, _airTankData);
#endif
                return;
            }

            CurrentAirTankCapacity = MaxAirTankCapacity;

            _cancellationTokenSource ??= new();
        }

        private void OnEnable()
        {
            AirRefillingStopped += OnAirRefillingStopped;
            AirTankEmptied += OnAirTankEmptied;
            AirTankFull += OnAirTankFull;
            AirTankRecharging += OnAirTankRecharging;
        }


        private void OnDisable()
        {
            AirTankRecharging -= OnAirTankRecharging;
            AirTankFull -= OnAirTankFull;
            AirTankEmptied -= OnAirTankEmptied;
            AirRefillingStopped -= OnAirRefillingStopped;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }
        #endregion


        #region Callback Functions
        private async void OnAirRefillingStopped()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_airTankData.AirTankRechargeDelay), ignoreTimeScale: false);
            AirTankRecharging?.Invoke();
        }

        private async void OnAirTankEmptied()
        {
            _isActive = false;
            await UniTask.Delay(TimeSpan.FromSeconds(_airTankData.AirTankRechargeDelay), ignoreTimeScale: false);
            AirTankRecharging?.Invoke();
        }

        private void OnAirTankFull()
        {
            _isActive = true;
            _isRecharging = false;
        }

        private void OnAirTankRecharging()
        {
            if(_isRecharging)
                return;

            _cancellationTokenSource.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();

            RechargeAirTank(_cancellationTokenSource.Token).Forget();
        }
        #endregion

        public override void Interact(Transform transform)
        {
            base.Interact(transform);

            if (!IsActivatable)
                return;

            if (!transform.gameObject.TryGetComponent(out AirSupply airSupply))
            {
#if UNITY_EDITOR
                Debug.LogErrorFormat("{1} is not assigned for {0}", transform.gameObject.name, airSupply);
#endif
                return;
            }

            if (_isInUse)
            {
                _cancellationTokenSource.Cancel();
                AirRefillingStopped?.Invoke();
                return;
            }

            RefillAirSupply(airSupply, _cancellationTokenSource.Token).Forget();
        }

        private async UniTask RefillAirSupply(AirSupply airSupply, CancellationToken token)
        {
            _isInUse = true;
            airSupply.SetAirConsumptionState(false);

            try
            {
                while (CurrentAirTankCapacity > 0.0f && airSupply.AirSupplyData.CurrentAirSupply < airSupply.AirSupplyData.MaxAirSupply)
                {
                    if (token.IsCancellationRequested)
                        break;

                    float refillAmount = _airTankData.DefaultAirRefillRate * Time.deltaTime;
                    airSupply.AirSupplyData.CurrentAirSupply += refillAmount;
                    CurrentAirTankCapacity -= refillAmount;
                    await UniTask.Yield();
                }
            }
            catch (OperationCanceledException)
            {
                // Handle exceptions if necessary
            }
            finally
            {
                _isInUse = false;
                airSupply.SetAirConsumptionState(true);
            }
        }

        private async UniTask RechargeAirTank(CancellationToken token)
        {
            _isRecharging = true;

            while (CurrentAirTankCapacity < MaxAirTankCapacity)
            {
                if (token.IsCancellationRequested)
                    break;

                CurrentAirTankCapacity += _airTankData.AirTankRechargeRate * Time.deltaTime;
                await UniTask.Yield();
            }
        }
    }
}