using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace OxygenSupplySystem
{
    //?Question: StartRefillProcessAsync: Use another waiting method instead of WaitForEndOfFrame()?

    /// <summary>
    /// Service to handle oxygen refilling processes between an OxygenSupply and an OxygenTankManager.
    /// </summary>
    public class OxygenSupplyRefillService : IDisposable
    {
        // Private Members
        private bool _disposedValue;
        private CancellationTokenSource _refillProcessCTS = null;


        #region Events

        public event Action OxygenRefillStarted;

        public event Action OxygenRefillStopped;

        #endregion


        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public async UniTask StartRefillProcessAsync(
            OxygenSupply oxygenSupply,
            OxygenTankManager oxygenTank,
            CancellationToken externalToken = default)
        {
            if (oxygenSupply == null || oxygenTank is null)
            {
#if UNITY_EDITOR
                Debug.LogErrorFormat("{0}: StartRefillProcessAsync: {1} and/or {2} are null! Aborting refill process.", nameof(OxygenSupplyRefillService), nameof(OxygenSupply), nameof(OxygenTankManager));
#endif
                return;
            }

            if (!oxygenTank.IsReady)
            {
#if UNITY_EDITOR
                Debug.LogFormat("{0}: StartRefillProcessAsync: {1} is not ready. Aborting refill process.", nameof(OxygenSupplyRefillService), nameof(OxygenTankManager));
#endif
                return;
            }

            // Create new CTS immediately so other methods see it's not null
            var newCTS = CancellationTokenSource.CreateLinkedTokenSource(externalToken);
            // Store the reference and null it immediately to prevent concurrent access
            var oldCTS = Interlocked.Exchange(ref _refillProcessCTS, newCTS);

            await CleanUpCTSAsync(oldCTS);

            var linkedToken = _refillProcessCTS.Token;

            OxygenRefillStarted?.Invoke();
            // Temporarily stop air consumption while refilling to ensure accurate refill amount.
            oxygenSupply.SetOxygenConsumptionState(false);

            try
            {
                var releasedAmount = 0f;

                while (oxygenTank.IsReady &&
                       oxygenSupply.OxygenLevel < oxygenSupply.MaxOxygenLevel &&
                       !linkedToken.IsCancellationRequested)
                {
                    if (!oxygenTank.TryReleaseOxygen(out releasedAmount))
                    {
                        break;
                    }

                    oxygenSupply.OxygenLevel += releasedAmount;

                    await UniTask.WaitForEndOfFrame(linkedToken);
                }

                // Resume oxygen consumption after refilling.
                oxygenSupply.SetOxygenConsumptionState(true);

                if (oxygenTank.IsReady)
                {
                    await oxygenTank.StartRechargingWithDelay();
                }

                linkedToken.ThrowIfCancellationRequested();
            }
            catch (OperationCanceledException ex)
            {
#if UNITY_EDITOR
                Debug.LogFormat("Oxygen refill process was stopped manually: {0}", ex.Message);
#endif
            }
            finally
            {
                OxygenRefillStopped?.Invoke();
                _refillProcessCTS?.Dispose();
            }
        }

        public bool StopRefillProcess()
        {
            if (_refillProcessCTS is null || _refillProcessCTS.IsCancellationRequested)
            {
                return false;
            }

            _refillProcessCTS.Cancel();
            return true;
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _refillProcessCTS?.Cancel();
                    _refillProcessCTS?.Dispose();
                    _refillProcessCTS = null;

                    OxygenRefillStopped = null;
                    OxygenRefillStarted = null;
                }

                _disposedValue = true;
            }
        }


        private async UniTask CleanUpCTSAsync(CancellationTokenSource cts)
        {
            if (cts is null)
                return;

            try
            {
                cts.Cancel();

                // Give other tasks time to observe the cancellation
                await UniTask.Yield();
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
    }
}