using UnityEngine;

namespace WwiseHelper
{
    public class WwiseEventResponder : ResponderBase
    {
        [SerializeField]
        private AK.Wwise.Event _akEvent;

        public override void Respond(IAmTriggerable trigger)
        {
            _akEvent.Post(gameObject);
        }
    }
}
