using UnityEngine;

namespace WwiseHelper
{
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
                foreach (var t in _triggers)
                {
                    t.Interface.ChangeTriggerable();
                }
            }
        }
    }
}
