using Cysharp.Threading.Tasks;
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
        #region Members
        
        // Serialized Fields
        [Header("References")]
        [SerializeField]
        private SOOxygenTankData _oxygenTankData = null;

        // Private Members
        private OxygenSupplyRefillService _refillService = null;

        private CancellationTokenSource _disableCTS = new();
        private CancellationTokenSource _refillProcessCTS = new();

        #endregion


        #region Properties

        public CancellationToken DisableCancellationToken => _disableCTS.Token;
        public OxygenTankManager OxygenTank { get; private set; } = null;

        #endregion


        #region Unity Lifecycle Methods

        protected override void Awake()
        {
            InitOxygenTank();
            InitRefillService();
            base.Awake();
        }

        protected override void OnEnable()
        {
            _disableCTS ??= new();
            base.OnEnable();
            SubscribeToEvents();
        }

        protected override void OnDisable()
        {
            UnsubscribeFromEvents();
            base.OnDisable();
            _disableCTS.TryCancel();
            _disableCTS.TryDispose();
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
                // Cancel any ongoing refill process before starting a new one.
                _refillProcessCTS.TryCancel();
                // Create a new linked token source to ensure proper cancellation on disable or destroy.
                _refillProcessCTS = CancellationTokenSource.CreateLinkedTokenSource(
                                                            this.destroyCancellationToken, 
                                                            this.DisableCancellationToken);

                var refillProcessToken = _refillProcessCTS.Token;

                _refillService.StartRefillProcessAsync(airSupply, OxygenTank, refillProcessToken).Forget();
            }
            else
            {
                _refillProcessCTS.TryCancel();
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

        private void TurnResponderOff()
        {
            // Automatically turn off the responder when the tank starts recharging.
            _currentState = ResponderState.Off;
        }

        #endregion


        #region Private Methods

        private void SubscribeToEvents()
        {
            OxygenTank.Emptied += TurnResponderOff;
            OxygenTank.StartedRecharging += TurnResponderOff;
        }

        private void UnsubscribeFromEvents()
        {
            OxygenTank.StartedRecharging -= TurnResponderOff;
            OxygenTank.Emptied -= TurnResponderOff;
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