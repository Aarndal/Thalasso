using UnityEngine;

public class UITextFadeResponder : Responder
{
    [SerializeField]
    private UITextFade _textFade = default;
    [SerializeField, TextArea]
    private string _text = default;
    [SerializeField, Min(0.001f)]
    private float _delayBetweenLetters;
    [SerializeField, Min(0.001f)]
    private float _delayBetweenWords;

    public override void Respond(GameObject triggeringObject, ResponderState responderState)
    {
        //_textFade.Fade();

        _textFade.TypeWriter(_delayBetweenLetters, _delayBetweenWords, _text);
    }
}
