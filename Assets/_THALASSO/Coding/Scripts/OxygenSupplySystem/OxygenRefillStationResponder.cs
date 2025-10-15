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
        private IConsumeOxygen _oxygenSupply = null;

        private CancellationTokenSource _disableCTS = new();
        private CancellationTokenSource _refillProcessCTS = new();

        #endregion


        #region Properties

        public ResponderState CurrentState
        {
            get => _currentState;
            private set
            {
                _currentState = value switch
                {
                    ResponderState.Off => ResponderState.Off,
                    ResponderState.On => ResponderState.On,
                    ResponderState.Switch => _currentState == ResponderState.Off ? ResponderState.On : ResponderState.Off,
                    _ => ResponderState.Off,
                };
            }
        }
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

            if (_disableCTS.TryCancelAndDispose())
            {
                _disableCTS = null;
            }
        }

        #endregion


        #region Public Methods

        public override void Respond(GameObject triggeringObject, ResponderState responderState)
        {
            if (!triggeringObject.TryGetComponent(out _oxygenSupply))
            {
#if UNITY_EDITOR
                Debug.LogErrorFormat("No {1} component found on {0}!", triggeringObject.name, nameof(IConsumeOxygen));
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
                if (TurnOff())
                    TurnOn();
            }
            else
            {
                TurnOff();
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

        private void SetCurrentResponderOff()
        {
            CurrentState = ResponderState.Off;
        }

        #endregion


        #region Private Methods

        private void SubscribeToEvents()
        {
            OxygenTank.Emptied += SetCurrentResponderOff;
            OxygenTank.StartedRecharging += SetCurrentResponderOff;
        }

        private void UnsubscribeFromEvents()
        {
            OxygenTank.StartedRecharging -= SetCurrentResponderOff;
            OxygenTank.Emptied -= SetCurrentResponderOff;
        }

        private bool TurnOff()
        {
            _refillProcessCTS.TryCancel(out var result);

            // Return true if cancelling the cts was successful or if it was already cancelled.
            return (result.Status == CancellationTokenSourceExtensions.OperationStatus.Success ||
                result.Status == CancellationTokenSourceExtensions.OperationStatus.AlreadyCancelled);
        }

        private void TurnOn()
        {
            // Create a new linked token source to ensure proper cancellation on disable or destroy.
            _refillProcessCTS = CancellationTokenSource.CreateLinkedTokenSource(
                                                        this.destroyCancellationToken,
                                                        this.DisableCancellationToken);

            var refillProcessToken = _refillProcessCTS.Token;

            _refillService.StartRefillProcessAsync(_oxygenSupply, OxygenTank, refillProcessToken).Forget();
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

            CurrentState = responderState;
            return true;
        }

        #endregion
    }
}