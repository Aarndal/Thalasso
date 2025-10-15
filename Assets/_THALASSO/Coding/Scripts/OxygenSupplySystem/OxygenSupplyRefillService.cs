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

            try
            {
                // Create new CTS immediately so other methods see it's not null
                var newCTS = CancellationTokenSource.CreateLinkedTokenSource(externalToken);
                // Store the reference and null it immediately to prevent concurrent access
                var oldCTS = Interlocked.Exchange(ref _refillProcessCTS, newCTS);

                CancellationTokenSourceExtensions.TryCancel(oldCTS);

                var linkedToken = _refillProcessCTS.Token;

                OxygenRefillStarted?.Invoke();
                // Temporarily stop air consumption while refilling to ensure accurate refill amount.
                oxygenSupply.SetOxygenConsumptionState(false);


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
                    await oxygenTank.StartRechargingWithDelay(linkedToken);
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
                CancellationTokenSourceExtensions.TryCancel(_refillProcessCTS);
            }
        }

        public bool StopRefillProcess()
        {
            return CancellationTokenSourceExtensions.TryCancel(_refillProcessCTS);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    CancellationTokenSourceExtensions.TryCancel(_refillProcessCTS);
                    _refillProcessCTS = null;

                    OxygenRefillStopped = null;
                    OxygenRefillStarted = null;
                }

                _disposedValue = true;
            }
        }
    }
}
