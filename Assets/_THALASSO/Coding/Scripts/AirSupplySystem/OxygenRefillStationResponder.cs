using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace AirSupplySystem
{
    /// <summary>
    /// Manages the interaction between an oxygen tank and an air supply system.
    /// Sets up the necessary services that handle the refilling process.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class OxygenRefillStationResponder : Responder
    {
        // Serialized Fields
        [Header("References")]
        [SerializeField]
        private SOOxygenTankData _oxygenTankData = null;

        // Private Members
        private OxygenSupplyRefillService _refillService = null;
        private CancellationTokenSource _refillProcessCTS = null;

        // Properties
        public OxygenTankManager OxygenTank { get; private set; } = null;


        #region Unity Lifecycle Methods
        protected override void Awake()
        {
            base.Awake();

            if (_oxygenTankData == null)
            {
#if UNITY_EDITOR
                Debug.LogErrorFormat("{1} is not assigned for {0}", gameObject.name, nameof(SOOxygenTankData));
#endif
                return;
            }

            OxygenTank = new(_oxygenTankData);

            _refillService = new();
            _refillProcessCTS ??= new();
        }

        private void OnDestroy()
        {
            _refillProcessCTS?.Cancel();

            _refillProcessCTS?.Dispose();
            _refillService?.Dispose();
            OxygenTank?.Dispose();

            _refillProcessCTS = null;
            _refillService = null;
            OxygenTank = null;
        }
        #endregion


        public override void Respond(GameObject triggeringObject, ResponderState responderState)
        {
            if (!triggeringObject.TryGetComponent(out OxygenSupply airSupply))
            {
#if UNITY_EDITOR
                Debug.LogErrorFormat("No {1} component found on {0}!", triggeringObject.name, nameof(OxygenSupply));
#endif
                return;
            }

            if (!TrySetCurrentState(responderState))
            {
                return;
            }

            if (_currentState == ResponderState.On)
            {
                _refillService.StartRefillProcessAsync(airSupply, OxygenTank, _refillProcessCTS.Token).Forget();
            }
            else
            {
                if (_refillService.StopRefillProcess())
                {
                    _refillProcessCTS?.Cancel();
                    _refillProcessCTS?.Dispose();
                    _refillProcessCTS = new CancellationTokenSource();
                }
            }
        }


        /// <summary>
        /// Will attempt to set the current state of the Responder.
        /// If the OxygenRefillStation is currently recharging, it will not allow state changes.
        /// If the requested state is the same as the current state (except for Switch state), it will not change.
        /// If the requested state is not defined and the current state is Off, it will not change, else it will switch to Off.
        /// </summary>
        /// <param name="responderState"></param>
        /// <returns></returns>
        private bool TrySetCurrentState(ResponderState responderState)
        {
            // Cannot change state while recharging or empty.
            if (!OxygenTank.IsReady)
                return false;

            // No state change needed.
            if (responderState != ResponderState.Switch && responderState == _currentState)
                return false;

            // If the requested state is None and the current state is Off, do nothing.
            if (responderState == ResponderState.None && _currentState == ResponderState.Off)
                return false;

            _currentState = responderState switch
            {
                ResponderState.Off => ResponderState.Off,
                ResponderState.On => ResponderState.On,
                ResponderState.Switch => _currentState == ResponderState.Off ? ResponderState.On : ResponderState.Off,
                _ => ResponderState.Off,
            };
            return true;
        }
    }
}