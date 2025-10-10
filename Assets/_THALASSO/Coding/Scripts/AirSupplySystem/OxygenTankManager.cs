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
        /// <summary>
        /// Represents the operational state of the oxygen tank.
        /// </summary>
        public enum TankState
        {
            Ready,      // Tank has oxygen and is ready to dispense
            Empty,      // Tank is completely empty
            Cooling,    // Tank is in cooldown period before recharging
            Recharging, // Tank is actively recharging
            Full        // Tank is at maximum capacity
        }


        #region Private Constants and Members
        private const float DEFAULT_RELEASE_AMOUNT = 0.0f;

        private readonly SOOxygenTankData _data;
        private readonly float _minCapacityThreshold = 0.001f;

        private CancellationTokenSource _internalCTS;
        private float _currentCapacity;
        private bool _disposedValue;
        private TankState _currentState;
        #endregion


        #region Events
        /// <summary>
        /// Triggered when capacity changes with oldValue, newValue, and filling degree.
        /// </summary>
        public event Action<float, float, float> CapacityChanged;

        /// <summary>
        /// Triggered when tank becomes empty.
        /// </summary>
        public event Action Emptied;

        /// <summary>
        /// Triggered when tank becomes full.
        /// </summary>
        public event Action Full;

        /// <summary>
        /// Triggered when recharging process starts.
        /// </summary>
        public event Action StartedRecharging;

        /// <summary>
        /// Triggered when cooling period starts.
        /// </summary>
        public event Action StartedCooling;

        /// <summary>
        /// Triggered when tank state changes.
        /// </summary>
        public event Action<TankState, TankState> StateChanged;
        #endregion

        #region Properties

        /// <summary>
        /// Current oxygen capacity of the tank.
        /// </summary>
        public float CurrentCapacity
        {
            get => _currentCapacity;
            private set
            {
                // Make sure the value is within valid bounds.
                var clampedValue = Mathf.Clamp(value, 0.0f, _data.MaxCapacity);

                // If the value is approximately the same as the current capacity, do nothing.
                if (Mathf.Approximately(_currentCapacity, clampedValue))
                    return;

                float oldValue = _currentCapacity;
                _currentCapacity = clampedValue;

                // Notify about the capacity change
                CapacityChanged?.Invoke(oldValue, _currentCapacity, FillingDegree);

                // Update tank state based on new capacity
                UpdateTankState();
            }
        }

        /// <summary>
        /// Current filling percentage of the tank (0.0 to 1.0).
        /// </summary>
        public float FillingDegree => _currentCapacity / _data.MaxCapacity;

        /// <summary>
        /// Current operational state of the tank.
        /// </summary>
        public TankState CurrentState => _currentState;

        /// <summary>
        /// Whether the tank is currently in the recharging process.
        /// </summary>
        public bool IsRecharging => _currentState == TankState.Recharging || _currentState == TankState.Cooling;

        #endregion

        #region Constructor and Initialization

        /// <summary>
        /// Creates a new oxygen tank manager with the specified data.
        /// </summary>
        /// <param name="data">Configuration data for the tank.</param>
        /// <exception cref="ArgumentNullException">Thrown if data is null.</exception>
        public OxygenTankManager(SOOxygenTankData data)
        {
            if(data == null)
                throw new ArgumentNullException(nameof(data), "Oxygen tank data cannot be null!");

            _data = data;
            _currentCapacity = data.MaxCapacity;
            _currentState = TankState.Full;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Attempts to manually start the recharging process for the oxygen tank.
        /// </summary>
        /// <param name="externalToken">Optional cancellation token to cancel the operation.</param>
        /// <returns>True if recharging was started, false if already recharging or full.</returns>
        public bool TryStartRecharging(CancellationToken externalToken = default)
        {
            if (IsRecharging || CurrentState == TankState.Full)
                return false;

            StartRechargingWithDelay(externalToken).Forget();
            return true;
        }

        /// <summary>
        /// Attempts to cancel any ongoing recharging process.
        /// </summary>
        /// <returns>True if a recharging process was cancelled, false otherwise.</returns>
        public bool TryCancelRecharging()
        {
            if (!IsRecharging)
                return false;

            _internalCTS?.Cancel();
            return true;
        }

        /// <summary>
        /// Attempts to release oxygen from the tank.
        /// </summary>
        /// <param name="releasedAmount">Amount of oxygen released if successful.</param>
        /// <param name="customReleaseRate">Optional custom release rate override.</param>
        /// <returns>True if oxygen was released, false if tank is recharging or empty.</returns>
        public bool TryReleaseOxygen(out float releasedAmount, float? customReleaseRate = null)
        {
            // Cannot consume oxygen while recharging.
            if (IsRecharging)
            {
#if UNITY_EDITOR
                Debug.LogError("Cannot release oxygen while the tank is recharging or cooling.");
#endif
                releasedAmount = DEFAULT_RELEASE_AMOUNT;
                return false;
            }

            // Use custom rate if provided, otherwise use default
            float airRefillRate = customReleaseRate ?? _data.DefaultAirRefillRate;

            // Consume the full refill rate if enough oxygen is available.
            if (CurrentCapacity > airRefillRate)
            {
                releasedAmount = airRefillRate;
                CurrentCapacity -= airRefillRate;
                return true;
            }

            // Consume the remaining oxygen if less than the refill rate.
            if (CurrentCapacity > _minCapacityThreshold)
            {
                releasedAmount = CurrentCapacity;
                CurrentCapacity = 0.0f;
                return true;
            }

            // Tank is empty
            releasedAmount = DEFAULT_RELEASE_AMOUNT;
            return false;
        }

        /// <summary>
        /// Dispose of resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Updates the tank state based on current capacity.
        /// </summary>
        private void UpdateTankState()
        {
            TankState newState = _currentState;

            // Determine new state based on capacity
            if (_currentCapacity <= _minCapacityThreshold && _currentState != TankState.Cooling && _currentState != TankState.Recharging)
            {
                newState = TankState.Empty;
                Emptied?.Invoke();

                // Auto-start recharging when empty
                StartRechargingWithDelay(CancellationToken.None).Forget();
            }
            else if (Mathf.Approximately(_currentCapacity, _data.MaxCapacity) && _currentState != TankState.Full)
            {
                newState = TankState.Full;
                Full?.Invoke();
            }
            else if (_currentCapacity > _minCapacityThreshold && _currentCapacity < _data.MaxCapacity &&
                     _currentState != TankState.Cooling && _currentState != TankState.Recharging)
            {
                newState = TankState.Ready;
            }

            // If state changed, update and notify
            if (newState != _currentState)
            {
                StateChanged?.Invoke(_currentState, newState);
                _currentState = newState;
            }
        }

        /// <summary>
        /// Starts the recharging process for the oxygen tank with a cooldown delay.
        /// </summary>
        /// <param name="externalToken">External cancellation token to link with internal operations.</param>
        private async UniTask StartRechargingWithDelay(CancellationToken externalToken)
        {
            if (IsRecharging || CurrentCapacity >= _data.MaxCapacity)
                return;

            // Clean up any existing CTS
            if (_internalCTS != null)
            {
                _internalCTS.Cancel();
                _internalCTS.Dispose();
            }

            _internalCTS = CancellationTokenSource.CreateLinkedTokenSource(externalToken);
            var linkedToken = _internalCTS.Token;

            try
            {
                // Calculate cooldown based on filling degree
                var rechargeCooldownModifier = Mathf.Clamp(1.0f - FillingDegree, 0.0f, 1.0f);
                var cooldownDuration = _data.FullRechargeCooldown * rechargeCooldownModifier;

                // Enter cooling state
                StartedCooling?.Invoke();
                StateChanged?.Invoke(_currentState, TankState.Cooling);
                _currentState = TankState.Cooling;

                // Await the cooldown period before starting the recharge process
                await UniTask.Delay(
                    delayTimeSpan: TimeSpan.FromSeconds(cooldownDuration),
                    cancellationToken: linkedToken);

                await RechargeOxygenAsync(linkedToken);
            }
            catch (OperationCanceledException ex)
            {
#if UNITY_EDITOR
                Debug.LogWarningFormat("Oxygen tank recharge process was cancelled: {0}", ex.Message);
#endif
                // Restore appropriate state based on current capacity
                UpdateTankState();
            }
            finally
            {
                if (_internalCTS != null)
                {
                    _internalCTS.Dispose();
                    _internalCTS = null;
                }
            }
        }

        /// <summary>
        /// Recharges the oxygen tank over time until it reaches maximum capacity or the process is cancelled.
        /// </summary>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        private async UniTask RechargeOxygenAsync(CancellationToken cancellationToken)
        {
            if (_currentState == TankState.Recharging)
                return;

            StartedRecharging?.Invoke();
            StateChanged?.Invoke(_currentState, TankState.Recharging);
            _currentState = TankState.Recharging;

            try
            {
                // Calculate time-based values once outside the loop
                float rechargeRatePerFrame = _data.RechargeRate;

                // Recharge until full or cancelled.
                while (!cancellationToken.IsCancellationRequested && CurrentCapacity < _data.MaxCapacity)
                {
                    CurrentCapacity += rechargeRatePerFrame * Time.deltaTime;
                    await UniTask.Yield(cancellationToken);
                }

                cancellationToken.ThrowIfCancellationRequested();

                // Ensure the capacity is set to max if fully recharged
                if (!cancellationToken.IsCancellationRequested)
                {
                    CurrentCapacity = _data.MaxCapacity;
                }
            }
            catch (OperationCanceledException ex)
            {
#if UNITY_EDITOR
                Debug.LogWarningFormat("Oxygen tank recharging was cancelled: {0}", ex.Message);
#endif
            }
            finally
            {
                // Update state based on current capacity
                UpdateTankState();
            }
        }

        /// <summary>
        /// Handles resource cleanup.
        /// </summary>
        /// <param name="disposing">Whether to dispose managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // Clean up managed resources
                    _internalCTS?.Cancel();
                    _internalCTS?.Dispose();
                    _internalCTS = null;

                    // Clear all event handlers
                    CapacityChanged = null;
                    Emptied = null;
                    Full = null;
                    StartedRecharging = null;
                    StartedCooling = null;
                    StateChanged = null;
                }

                _disposedValue = true;
            }
        }

        #endregion
    }
}