using UnityEngine;

public class WwiseEventResponder : ResponderBase
{
    [SerializeField]
    private AK.Wwise.Event _akEvent;

    private bool _akEventIsPlaying = false;

    public override void Respond(IAmTriggerable trigger) // Change TransferParameter to Transform
    {
        if (!_akEventIsPlaying)
        {
            _akEvent.Post(gameObject);
            _akEventIsPlaying = true;
        }
    }
}
