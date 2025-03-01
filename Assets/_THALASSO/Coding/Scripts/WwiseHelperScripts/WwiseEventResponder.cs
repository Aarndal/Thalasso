using UnityEngine;

namespace WwiseHelper
{
    [RequireComponent(typeof(AkGameObj))]
    public class WwiseEventResponder : ResponderBase
    {
        [SerializeField]
        private bool _isOneTimeEvent = true;
        [SerializeField]
        private AK.Wwise.Event _akEvent;

        public override void Respond(IAmTriggerable trigger)
        {
            _akEvent.Post(gameObject);

            if (_isOneTimeEvent)
            {
                foreach (var triggerable in _triggers)
                {
                    if (triggerable.Interface.IsTriggerable)
                        triggerable.Interface.ChangeIsTriggerable();
                }
            }
        }
    }
}
