using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace AirSupplySystem
{
    /// <summary>
    /// Service to handle air refilling processes between an AirSupply and an OxygenTankManager.
    /// </summary>
    public class AirRefillService : IDisposable
    {
        // Private Members
        private bool _disposedValue;
        private CancellationTokenSource _internalCTS = null;

        // Events
        public event Action AirRefillStarted;
        public event Action AirRefillStopped;


        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public async UniTask StartRefillProcessAsync(
            AirSupply airSupply,
            OxygenTankManager oxygenTank,
            CancellationToken externalToken)
        {
            if (airSupply == null || oxygenTank is null)
            {
                throw new ArgumentNullException("AirSupply and OxygenTankManager cannot be null.");
            }

            if (oxygenTank.IsInRechargeProcess)
            {
#if UNITY_EDITOR
                Debug.LogWarning("Cannot refill air supply while the oxygen tank is recharging.");
#endif
                return;
            }

            // Cancel any existing refill process before starting a new one.
            _internalCTS?.Cancel();
            _internalCTS = CancellationTokenSource.CreateLinkedTokenSource(externalToken);
            var linkedToken = _internalCTS.Token;
            
            AirRefillStarted?.Invoke();

            // Temporarily stop air consumption while refilling to ensure accurate refill amount.
            airSupply.SetAirConsumptionState(false);

            try
            {
                var releasedAmount = 0f;
                while (airSupply.AirSupplyData.CurrentAirSupply < airSupply.MaxAirSupply &&
                       !linkedToken.IsCancellationRequested)
                {
                    if (!oxygenTank.TryReleaseOxygen(out releasedAmount))
                        break;
                    
                    airSupply.AirSupplyData.CurrentAirSupply += releasedAmount;

                    await UniTask.Yield(linkedToken);
                }

                AirRefillStopped?.Invoke();
                linkedToken.ThrowIfCancellationRequested();
            }
            catch (OperationCanceledException ex)
            {
#if UNITY_EDITOR
                Debug.LogWarningFormat("Air refill process was stopped manually: {0}", ex.Message);
#endif
            }
            finally
            {
                airSupply.SetAirConsumptionState(true);
                _internalCTS?.Dispose();
            }
        }

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
                
                _disposedValue = true;
            }
        }
    }
}