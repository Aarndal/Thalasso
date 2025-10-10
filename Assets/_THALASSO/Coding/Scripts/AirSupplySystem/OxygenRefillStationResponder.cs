using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace AirSupplySystem
{
    [DisallowMultipleComponent]
    public sealed class OxygenRefillStationResponder : Responder
    {
        // Serialized Fields
        [Header("References")]
        [SerializeField]
        private SOOxygenTankData _oxygenTankData = null;

        // Private Members
        private bool _isRecharging = false;
        private CancellationTokenSource _cancellationTokenSource = null;

        private AirRefillService _airRefillService = null;
        public OxygenTankManager TankManager = null;

        // Events
        public event Action AirRefillingStopped;

        #region Unity Lifecycle Methods
        protected override void Awake()
        {
            base.Awake();

            if (_oxygenTankData == null)
            {
                Debug.LogError($"OxygenTankData is not assigned for {gameObject.name}", this);
                return;
            }

            _cancellationTokenSource ??= new();

            TankManager = new OxygenTankManager(_oxygenTankData);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            SubscribeToEvents();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            UnsubscribeFromEvents();
        }

        private void OnDestroy()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }
        #endregion


        #region Callback Functions
        private async void OnAirRefillingStopped()
        {
            await TankManager.RechargeOxygenAsync(_cancellationTokenSource.Token);
        }

        private async void OnOxygenTankEmptied()
        {
            await TankManager.RechargeOxygenAsync(_cancellationTokenSource.Token);
        }

        private void OnOxygenTankFull()
        {
            _isRecharging = false;
        }
        #endregion


        #region Public Methods
        public override void Respond(GameObject triggeringObject, ResponderState responderState)
        {
            if (!triggeringObject.TryGetComponent(out AirSupply airSupply))
            {
                Debug.LogError($"No {nameof(AirSupply)} component found on {triggeringObject.name}", this);
                return;
            }

            if (_isRecharging)
            {
                return;
            }

            if (!TrySetCurrentState(responderState))
            {
                return;
            }

            if (_currentState == ResponderState.On)
                _airRefillService.RefillAirSupplyFromTankAsync(airSupply, TankManager, _cancellationTokenSource.Token).Forget();
            else
                StopAirRefillingProcess();
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Cancels any ongoing air refilling process and sets the Responder state to Off.
        /// </summary>
        private void StopAirRefillingProcess()
        {
            if (_currentState != ResponderState.Off)
                TrySetCurrentState(ResponderState.Off);

            _cancellationTokenSource?.Cancel();

            AirRefillingStopped?.Invoke();
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
            if (_isRecharging)
            {
#if UNITY_EDITOR
                Debug.LogWarningFormat("{0} is currently recharging and cannot be used.", gameObject.name);
#endif
                return false;
            }

            if (responderState != ResponderState.Switch && responderState == _currentState)
                return false;

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



        // Event Subscription Methods
        private void SubscribeToEvents()
        {
            AirRefillingStopped += OnAirRefillingStopped;
            OxygenTankEmptied += OnOxygenTankEmptied;
            OxygenTankFull += OnOxygenTankFull;
        }

        private void UnsubscribeFromEvents()
        {
            OxygenTankFull -= OnOxygenTankFull;
            OxygenTankEmptied -= OnOxygenTankEmptied;
            AirRefillingStopped -= OnAirRefillingStopped;
        }
        #endregion
    }
}