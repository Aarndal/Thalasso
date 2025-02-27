using UnityEngine;

namespace WwiseHelper
{
    public class WwiseEventResponder : ResponderBase
    {
        [SerializeField]
        private AK.Wwise.Event _akEvent;

        private bool _akEventIsPlaying = false;

        public override void Respond(IAmTriggerable trigger)
        {
            if (!_akEventIsPlaying)
            {
                _akEvent.Post(gameObject);
                _akEventIsPlaying = true;
            }
        }
    }
}
