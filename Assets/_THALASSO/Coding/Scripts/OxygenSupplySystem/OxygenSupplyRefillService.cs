using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using Unity.VisualScripting;
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
        private CancellationTokenSource _refillProcessCTS = new();


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
            IConsumeOxygen oxygenSupply,
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

            oldCTS?.TryCancelAndDispose();

            var linkedToken = _refillProcessCTS.Token;

            try
            {
                // Temporarily stop oxygen consumption while refilling to ensure accurate refill amount.
                if (!oxygenSupply.TryStopConsumingOxygen())
                    return;

                OxygenRefillStarted?.Invoke();

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

                linkedToken.ThrowIfCancellationRequested();
            }
            catch (OperationCanceledException ex) when (linkedToken.IsCancellationRequested)
            {
#if UNITY_EDITOR
                Debug.LogFormat("Oxygen refill process was stopped manually: {0}", ex.Message);
#endif
            }
            finally
            {
                // Resume oxygen consumption after refilling.
                oxygenSupply.TryStartConsumingOxygenAsync().Forget();

                if (oxygenTank.IsReady)
                {
                    oxygenTank.StartRechargingWithDelay(externalToken).Forget();
                }

                OxygenRefillStopped?.Invoke();
                _refillProcessCTS?.TryCancel();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    CancellationTokenSourceExtensions.TryCancelAndDispose(_refillProcessCTS);
                    _refillProcessCTS = null;

                    OxygenRefillStopped = null;
                    OxygenRefillStarted = null;
                }

                _disposedValue = true;
            }
        }
    }
}
