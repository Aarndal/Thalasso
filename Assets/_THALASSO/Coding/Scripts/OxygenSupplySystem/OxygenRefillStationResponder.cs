using Cysharp.Threading.Tasks;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

namespace OxygenSupplySystem
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
            InitOxygenTank();
            InitRefillService();
            base.Awake();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            SubscribeToEvents();
        }

        protected override void OnDisable()
        {
            UnsubscribeFromEvents();
            base.OnDisable();
        }

        private void OnDestroy()
        {
            Dispose();
        }

        private void Dispose()
        {
            CleanUpCTS(_refillProcessCTS);
            //_refillService?.Dispose();
            //OxygenTank?.Dispose();

            //await UniTask.WaitForSeconds(2.0f);

            _refillProcessCTS = null;
            //_refillService = null;
            //OxygenTank = null;
        }

        #endregion


        #region Public Methods

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
                var token = _refillProcessCTS.Token;
                _refillService.StartRefillProcessAsync(airSupply, OxygenTank, token).Forget();
            }
            else
            {
                _refillService.StopRefillProcess();
            }
        }

        #endregion


        #region Initialization Methods

        private void InitOxygenTank()
        {
            if (_oxygenTankData == null)
            {
#if UNITY_EDITOR
                Debug.LogErrorFormat("{1} is not assigned for {0}", gameObject.name, nameof(SOOxygenTankData));
#endif
            }
            else
            {
                OxygenTank = new(_oxygenTankData);
            }
        }

        private void InitRefillService()
        {
            _refillService = new();
            _refillProcessCTS ??= new();
        }

        #endregion


        #region Callback Functions

        private void ResetRefillProcessCTS()
        {
            // Create new CTS immediately so other methods see it's not null
            CancellationTokenSource newCTS = new();
            // Store the reference and null it immediately to prevent concurrent access
            var oldCTS = Interlocked.Exchange(ref _refillProcessCTS, newCTS);

            CleanUpCTS(oldCTS);
        }

        private void TurnResponderOff()
        {
            // Automatically turn off the responder when the tank starts recharging.
            _currentState = ResponderState.Off;
        }

        #endregion


        #region Private Methods

        private void SubscribeToEvents()
        {
            _refillService.OxygenRefillStopped += ResetRefillProcessCTS;
            OxygenTank.Emptied += TurnResponderOff;
            OxygenTank.StartedRecharging += TurnResponderOff;
        }

        private void UnsubscribeFromEvents()
        {
            OxygenTank.StartedRecharging -= TurnResponderOff;
            OxygenTank.Emptied -= TurnResponderOff;
            _refillService.OxygenRefillStopped -= ResetRefillProcessCTS;
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
            if (OxygenTank is null)
                return false;

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

        #endregion
    }
}