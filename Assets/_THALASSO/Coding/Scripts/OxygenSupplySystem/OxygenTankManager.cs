using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace OxygenSupplySystem
{
    //!TODO: TryReleaseOxygen: Variable release rate based on external factors.
    //?Question: RechargeOxygenAsync: Use another waiting method instead of WaitForEndOfFrame()?

    /// <summary>
    /// Manages the state changes and behavior of an oxygen tank, including release and recharging of oxygen.
    /// </summary>
    public class OxygenTankManager : IDisposable
    {
        /// <summary>
        /// Represents the operational state of the oxygen tank.
        /// </summary>
        private enum TankState
        {
            Full,       // Tank is at maximum capacity
            Empty,      // Tank is empty
            Pending,    // Tank is pending for release request or recharge
            Releasing,  // Tank is actively releasing oxygen
            Recharging, // Tank is actively recharging
        }


        #region Private Constants and Members

        //Constants
        private const float EMPTY_CAPACITY_THRESHOLD = 0.01f; // Threshold to consider the tank empty
        private const float ZERO_RELEASE_AMOUNT = 0.0f; // Release amount when no oxygen can be released

        // Private Members
        private readonly SOOxygenTankData _data = null;

        private float _currentCapacity = 0.0f;
        private TankState _currentState = TankState.Empty;
        private bool _disposedValue;
        private CancellationTokenSource _rechargeCTS;
        private CancellationTokenSource _cooldownCTS;

        #endregion


        #region Events

        /// <summary>
        /// Triggered when capacity changes with newValue, and filling degree.
        /// <remarks>Filling degree is a value between 0.0 and 1.0 representing the percentage of the tank that is filled.</remarks>
        /// </summary>
        public event Action<float, float> CapacityChanged;

        /// <summary>
        /// Triggered when tank is starting to release oxygen.
        /// </summary>
        public event Action StartedReleasingOxygen;

        /// <summary>
        /// Triggered when tank becomes empty.
        /// </summary>
        public event Action Emptied;

        /// <summary>
        /// Triggered when tank is emptied or the release process is canceled.
        /// </summary>
        public event Action StartedCooldown;

        /// <summary>
        /// Triggered when cooling period ends and recharging begins.
        /// </summary>
        public event Action StartedRecharging;

        /// <summary>
        /// Triggered when tank becomes full and stops recharging.
        /// </summary>
        public event Action StoppedRecharging;

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
                var clampedValue = Mathf.Clamp(value, 0.0f, _data.MaxCapacity);

                // If the value is approximately the same as the current capacity, do nothing.
                if (Mathf.Approximately(_currentCapacity, clampedValue))
                    return;

                _currentCapacity = clampedValue;
                CapacityChanged?.Invoke(_currentCapacity, FillingDegree);

                // If the tank is emptied, trigger the Emptied event and transition to Empty state.
                if (_currentCapacity <= EMPTY_CAPACITY_THRESHOLD)
                {
                    TryTransitionTo(TankState.Empty);
                    Emptied?.Invoke();
                    StartRechargingWithDelay().Forget();
                }

                if (Mathf.Approximately(_currentCapacity, _data.MaxCapacity))
                {
                    TryTransitionTo(TankState.Full);
                }
            }
        }

        /// <summary>
        /// Current filling percentage of the tank (0.0 to 1.0).
        /// </summary>
        public float FillingDegree => Mathf.Clamp(_currentCapacity / _data.MaxCapacity, 0.0f, 1.0f);

        /// <summary>
        /// Current operational state of the tank.
        /// </summary>
        public bool IsReady => (_currentState != TankState.Empty && _currentState != TankState.Recharging);

        #endregion


        #region Constructor and Initialization

        /// <summary>
        /// Creates a new oxygen tank manager with the specified data.
        /// </summary>
        /// <param name="data">Configuration data for the tank.</param>
        /// <exception cref="ArgumentNullException">Thrown if data is null.</exception>
        public OxygenTankManager(SOOxygenTankData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data), "Oxygen tank data cannot be null!");

            _data = data;
            _currentCapacity = data.MaxCapacity;
            _currentState = TankState.Full;
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// Starts the recharging process for the oxygen tank with a cooldown delay.
        /// </summary>
        /// <param name="externalToken">External cancellation token to link with internal operations.</param>
        public async UniTask StartRechargingWithDelay(CancellationToken externalToken = default)
        {
            if (_currentState == TankState.Recharging || _currentState == TankState.Full)
                return;

            // Create new CTS immediately so other methods see it's not null
            var newCTS = CancellationTokenSource.CreateLinkedTokenSource(externalToken);
            // Store the reference and null it immediately to prevent concurrent access
            var oldCTS = Interlocked.Exchange(ref _cooldownCTS, newCTS);

            CleanUpCTS(oldCTS);

            var linkedToken = _cooldownCTS.Token;

            try
            {
                linkedToken.ThrowIfCancellationRequested();

                if (_currentState != TankState.Pending && _currentState != TankState.Empty)
                {
                    TryTransitionTo(TankState.Pending);
                }

                StartedCooldown?.Invoke();

                linkedToken.ThrowIfCancellationRequested();

                // Calculate cooldown based on filling degree
                var rechargeCooldownModifier = Mathf.Clamp(1.0f - FillingDegree, 0.0f, 1.0f);
                var cooldownDuration = Mathf.Clamp(value: _data.MaxRechargeCooldown * rechargeCooldownModifier,
                                                        min: _data.MinRechargeCooldown,
                                                        max: _data.MaxRechargeCooldown);

                // Await the cooldown period before starting the recharge process
                await UniTask.Delay(
                    delayTimeSpan: TimeSpan.FromSeconds(cooldownDuration),
                    cancellationToken: linkedToken);

                linkedToken.ThrowIfCancellationRequested();

                await RechargeOxygenAsync(linkedToken);

                linkedToken.ThrowIfCancellationRequested();
            }
            catch (OperationCanceledException ex)
            {
#if UNITY_EDITOR
                Debug.LogWarningFormat("Oxygen tank recharge process was cancelled: {0}", ex.Message);
#endif
                if (CurrentCapacity > EMPTY_CAPACITY_THRESHOLD)
                {
                    TryTransitionTo(TankState.Pending);
                }
                else
                {
                    TryTransitionTo(TankState.Empty);
                }
            }
            finally
            {
                CleanUpCTS(_cooldownCTS);
            }
        }

        /// <summary>
        /// Attempts to release oxygen from the tank.
        /// </summary>
        /// <param name="releasedAmount">Amount of oxygen released if successful.</param>
        /// <returns>True if oxygen was released, false if tank is recharging or empty.</returns>
        public bool TryReleaseOxygen(out float releasedAmount)
        {
            releasedAmount = ZERO_RELEASE_AMOUNT;

            // Cannot consume oxygen while recharging or empty.
            if (_currentState == TankState.Recharging || _currentState == TankState.Empty)
            {
#if UNITY_EDITOR
                Debug.LogWarningFormat("Cannot release oxygen while the tank is recharging or empty.");
#endif
                return false;
            }

            if (_currentState != TankState.Releasing)
            {
                TryTransitionTo(TankState.Releasing);
                StartedReleasingOxygen?.Invoke();
            }

            //!TODO: Variable release rate based on external factors.
            float releaseRate = _data.DefaultReleaseRate;
            releasedAmount = releaseRate * Time.deltaTime;

            if (CurrentCapacity > releasedAmount)
            {
                CurrentCapacity -= releasedAmount;
                return true;
            }

            if (CurrentCapacity > EMPTY_CAPACITY_THRESHOLD)
            {
                releasedAmount = CurrentCapacity;
                CurrentCapacity = 0.0f;
                return true;
            }

            // The method should not reach this point if the tank is empty.
#if UNITY_EDITOR
            Debug.LogErrorFormat("{1} is not set to {2} although empty: {0}", this, nameof(OxygenTankManager), TankState.Empty.ToString());
#endif
            releasedAmount = ZERO_RELEASE_AMOUNT;
            CurrentCapacity = 0.0f;
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
        /// Updates the tank state.
        /// </summary>
        private bool TryTransitionTo(TankState newState)
        {
            if (_currentState == newState)
                return false;

            _currentState = newState;
            return true;
        }

        /// <summary>
        /// Recharges the oxygen tank over time until it reaches maximum capacity or the process is cancelled.
        /// </summary>
        /// <param name="externalToken">Token to monitor for cancellation requests.</param>
        private async UniTask RechargeOxygenAsync(CancellationToken externalToken = default)
        {
            if (_currentState == TankState.Recharging)
                return;

            // Create new CTS immediately so other methods see it's not null
            var newCTS = CancellationTokenSource.CreateLinkedTokenSource(externalToken);
            // Store the reference and null it immediately to prevent concurrent access
            var oldCTS = Interlocked.Exchange(ref _rechargeCTS, newCTS);

            CleanUpCTS(oldCTS);

            var linkedToken = _rechargeCTS.Token;

            try
            {
                linkedToken.ThrowIfCancellationRequested();

                TryTransitionTo(TankState.Recharging);
                StartedRecharging?.Invoke();

                float rechargeRatePerFrame = _data.RechargeRate;

                // Recharge until full or cancelled.
                while (!externalToken.IsCancellationRequested && _currentState != TankState.Full)
                {
                    CurrentCapacity += rechargeRatePerFrame * Time.deltaTime;
                    await UniTask.WaitForEndOfFrame(linkedToken);
                }

                linkedToken.ThrowIfCancellationRequested();

                // Ensure the capacity is set to max if fully recharged
                CurrentCapacity = _data.MaxCapacity;
            }
            catch (OperationCanceledException ex)
            {
#if UNITY_EDITOR
                Debug.LogWarningFormat("Oxygen tank recharging was cancelled: {0}", ex.Message);
#endif
            }
            finally
            {
                StoppedRecharging?.Invoke();

                CleanUpCTS(_rechargeCTS);
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
                    CleanUpCTS(_rechargeCTS);
                    _rechargeCTS = null;

                    CleanUpCTS(_cooldownCTS);
                    _cooldownCTS = null;

                    // Clear all event handlers
                    CapacityChanged = null;
                    Emptied = null;
                    StoppedRecharging = null;
                    StartedRecharging = null;
                    StartedCooldown = null;
                    StartedReleasingOxygen = null;
                }

                _disposedValue = true;
            }
        }

        private void CleanUpCTS(CancellationTokenSource cts)
        {
            if (cts is null)
                return;

            try
            {
                cts.Cancel();
            }
            catch (Exception ex)
            {
#if UNITY_EDITOR
                Debug.LogError($"Exception during CTS cleanup: {ex.Message}");
#endif
            }
            finally
            {
                cts.Dispose();
            }
        }

        #endregion
    }
}