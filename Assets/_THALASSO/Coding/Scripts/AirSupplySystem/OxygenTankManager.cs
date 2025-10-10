using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace AirSupplySystem
{
    /// <summary>
    /// Manages the state and behavior of an oxygen tank, including release and recharging of oxygen.
    /// </summary>
    public class OxygenTankManager : IDisposable
    {
        // Private Members
        private readonly SOOxygenTankData _data;

        private CancellationTokenSource _internalCTS = null;
        private float _currentCapacity;
        private bool _disposedValue;
        private bool _isRecharging;

        // Events
        public event Action<float> CapacityChanged;
        public event Action Emptied;
        public event Action Full;

        // Properties
        public float CurrentCapacity
        {
            get => _currentCapacity;
            set
            {
                // Make sure the value is within valid bounds.
                var clampedValue = Mathf.Clamp(value, 0.0f, _data.MaxCapacity);

                // If the value is approximately the same as the current capacity, do nothing.
                if (Mathf.Approximately(_currentCapacity, clampedValue))
                    return;

                _currentCapacity = clampedValue;
                CapacityChanged?.Invoke(_currentCapacity);

                // Trigger events based on the new capacity.
                if (Mathf.Approximately(_currentCapacity, 0.0f))
                {
                    Emptied?.Invoke();
                }
                else if (Mathf.Approximately(_currentCapacity, _data.MaxCapacity))
                {
                    Full?.Invoke();
                }
            }
        }
        public float FillingDegree => _currentCapacity / _data.MaxCapacity;
        public bool IsRecharging => _isRecharging;

        // Constructor
        public OxygenTankManager(SOOxygenTankData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data), "Oxygen tank data cannot be null!");

            _data = data;
            _currentCapacity = data.MaxCapacity;
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~OxygenTankManager()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }


        #region Public Methods
        /// <summary>
        /// Method to consume a released amount of oxygen from the tank.
        /// Can only be called if the tank is not currently recharging or empty.
        /// </summary>
        /// <param name="releasedAmount">The amount of oxygen that can be consumed.</param>
        /// <returns>True if oxygen can be consumed, false otherwise.</returns>
        public bool TryReleaseOxygen(out float releasedAmount)
        {
            // Cannot consume oxygen while recharging.
            if (_isRecharging)
            {
#if UNITY_EDITOR
                Debug.LogError("Cannot release oxygen while the tank is recharging.");
#endif
                releasedAmount = 0.0f;
                return false;
            }

            //TODO: Variable refill rate based on external factors.
            var airRefillRate = _data.DefaultAirRefillRate;

            // Consume the full refill rate if enough oxygen is available.
            if (CurrentCapacity > airRefillRate)
            {
                releasedAmount = airRefillRate;
                CurrentCapacity -= airRefillRate;
                return true;
            }

            // Consume the remaining oxygen if less than the refill rate.
            if (CurrentCapacity > 0.0f)
            {
                releasedAmount = CurrentCapacity;
                CurrentCapacity = 0.0f;
                return true;
            }

            // Tank is empty
            releasedAmount = 0.0f;
            return false;
        }

        /// <summary>
        /// Recharge the oxygen tank over time until it reaches maximum capacity or the process is cancelled.
        /// </summary>
        /// <param name="externalToken"></param>
        /// <returns></returns>
        public async UniTask RechargeOxygenAsync(CancellationToken externalToken)
        {
            if (_isRecharging)
                return;

            // Cancel any ongoing recharge process before starting a new one.
            _internalCTS?.Cancel();

            _isRecharging = true;

            // Create a linked CTS to consider both internal and external cancellation requests.
            _internalCTS = CancellationTokenSource.CreateLinkedTokenSource(externalToken);
            var linkedToken = _internalCTS.Token;

            try
            {
                //Question: Should the cooldown be affected by the filling degree?
                var rechargeCooldownModifier = Mathf.Clamp(1.0f - FillingDegree, 0.0f, 1.0f);

                // Await the cooldown period before starting the recharge process
                await UniTask.Delay(
                      delayTimeSpan: TimeSpan.FromSeconds(_data.FullRechargeCooldown * rechargeCooldownModifier),
                      cancellationToken: linkedToken);

                // Recharge until full or cancelled.
                while (!linkedToken.IsCancellationRequested && CurrentCapacity < _data.MaxCapacity)
                {
                    CurrentCapacity += _data.RechargeRate * Time.deltaTime;
                    await UniTask.Yield(linkedToken);
                }

                linkedToken.ThrowIfCancellationRequested();

                // Ensure the capacity is set to max if fully recharged.
                CurrentCapacity = _data.MaxCapacity;
            }
            catch (OperationCanceledException ex)
            {
#if UNITY_EDITOR
                Debug.LogWarningFormat("Oxygen tank recharge was cancelled: {0}", ex.Message);
#endif
            }
            finally
            {
                _isRecharging = false;
                _internalCTS?.Dispose();
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion


        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _internalCTS?.Cancel();
                    _internalCTS?.Dispose();
                    _internalCTS = null;
                }

                Full = null;
                Emptied = null;
                CapacityChanged = null;

                _disposedValue = true;
            }
        }
    }
}