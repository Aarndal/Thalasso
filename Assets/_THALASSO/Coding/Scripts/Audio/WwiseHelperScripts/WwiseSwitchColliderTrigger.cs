using UnityEngine;

namespace WwiseHelper
{
    public class WwiseSwitchColliderTrigger : ColliderTrigger
    {
#if WWISE_2024_OR_LATER
        [Header("Wwise Settings")]
        [SerializeField]
        private AK.Wwise.Switch _wwiseSwitch = default;

        public AK.Wwise.Switch AudioSwitch => _wwiseSwitch;

        public override void ActivateTrigger(GameObject triggeringGameObject, ResponderState responderState)
        {
            if (!IsValidTrigger(triggeringGameObject))
                return;

            if (IsTriggerable)
            {
                _wwiseSwitch.SetValue(triggeringGameObject);
                _isTriggered?.Invoke(gameObject, responderState);
            }
            else
            {
                _cannotBeTriggered?.Invoke(gameObject, _cannotBeTriggeredMessage);
            }
        }

        protected override bool IsValidTrigger(GameObject triggeringGameObject) => triggeringGameObject.TryGetComponent<AkBank>(out _);
#endif
    }
}
