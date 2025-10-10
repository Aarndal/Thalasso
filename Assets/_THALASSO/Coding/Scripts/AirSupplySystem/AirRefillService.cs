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
        private bool _disposedValue;
        private CancellationTokenSource _internalCTS = null;

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~AirRefillService()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }


        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public async UniTask RefillAirSupplyFromTankAsync(
            AirSupply airSupply,
            OxygenTankManager tankManager,
            CancellationToken externalToken)
        {
            if (airSupply == null || tankManager == null)
            {
                throw new ArgumentNullException("AirSupply and OxygenTankManager cannot be null.");
            }

            if (tankManager.IsRecharging)
            {
#if UNITY_EDITOR
                Debug.LogWarning("Cannot refill air supply while the oxygen tank is recharging.");
#endif
                return;
            }

            _internalCTS?.Cancel();

            // Temporarily stop air consumption while refilling to ensure accurate refill amount.
            airSupply.SetAirConsumptionState(false);

            _internalCTS = CancellationTokenSource.CreateLinkedTokenSource(externalToken);
            var linkedToken = _internalCTS.Token;

            try
            {
                var releasedAmount = 0f;
                while (airSupply.AirSupplyData.CurrentAirSupply < airSupply.MaxAirSupply &&
                       !linkedToken.IsCancellationRequested)
                {
                    if (!tankManager.TryReleaseOxygen(out releasedAmount))
                        break;
                    
                    airSupply.AirSupplyData.CurrentAirSupply += releasedAmount;

                    await UniTask.Yield(linkedToken);
                }
            }
            catch (OperationCanceledException ex)
            {
#if UNITY_EDITOR
                Debug.LogWarningFormat("Air refill process was cancelled: {0}", ex.Message);
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

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
            }
        }
    }
}