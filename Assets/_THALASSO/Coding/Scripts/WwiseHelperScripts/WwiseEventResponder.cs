using UnityEngine;

namespace WwiseHelper
{
#if WWISE_2024_OR_LATER
    [RequireComponent(typeof(AkGameObj))]
#endif
    public class WwiseEventResponder : ResponderBase
    {
#if WWISE_2024_OR_LATER

        [SerializeField]
        private bool _isOneTimeEvent = true;
        [SerializeField]
        private AK.Wwise.Event _akEvent;
#endif
        public override void Respond(GameObject @gameObject, IAmTriggerable trigger)
        {
#if WWISE_2024_OR_LATER

            _akEvent.Post(gameObject);

            if (_isOneTimeEvent)
            {
                foreach (var triggerable in _triggers)
                {
                    if (triggerable.Interface.IsTriggerable)
                        triggerable.Interface.ChangeIsTriggerable();
                }
            }
#endif
        }
    }
}
