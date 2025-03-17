using UnityEngine;

public class UITextFadeResponder : Responder
{
    [SerializeField]
    private UITextFade _textFade = default;

    public override void Respond(GameObject triggeringObject, ResponderState responderState)
    {
        _textFade.Fade();
    }
}
