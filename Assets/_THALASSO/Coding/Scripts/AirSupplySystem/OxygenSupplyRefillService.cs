using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace AirSupplySystem
{
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
                return;

            // Cancel any existing refill process before starting a new one.
            _refillProcessCTS?.Cancel();
            _refillProcessCTS = CancellationTokenSource.CreateLinkedTokenSource(externalToken);
            var linkedToken = _refillProcessCTS.Token;

            OxygenRefillStarted?.Invoke();
            // Temporarily stop air consumption while refilling to ensure accurate refill amount.
            oxygenSupply.SetOxygenConsumptionState(false);

            try
            {
                var releasedAmount = 0f;
                while (oxygenTank.IsReady &&
                       oxygenSupply.OxygenSupplyData.CurrentOxygenLevel < oxygenSupply.MaxOxygenLevel &&
                       !linkedToken.IsCancellationRequested)
                {
                    if (!oxygenTank.TryReleaseOxygen(out releasedAmount))
                    {
                        break;
                    }

                    oxygenSupply.OxygenSupplyData.CurrentOxygenLevel += releasedAmount;

                    await UniTask.Yield(linkedToken);
                }

                if (oxygenTank.IsReady)
                {
                    oxygenTank.StartRechargingWithDelay().Forget();
                }

                linkedToken.ThrowIfCancellationRequested();
            }
            catch (OperationCanceledException ex)
            {
#if UNITY_EDITOR
                Debug.LogWarningFormat("Oxygen refill process was stopped manually: {0}", ex.Message);
#endif
            }
            finally
            {
                oxygenSupply.SetOxygenConsumptionState(true);
                OxygenRefillStopped?.Invoke();
                _refillProcessCTS?.Dispose();
            }
        }

        public bool StopRefillProcess()
        {
            if (_refillProcessCTS == null || _refillProcessCTS.IsCancellationRequested)
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
    }
}