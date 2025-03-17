using System.Threading.Tasks;
using UnityEngine;

public class UITextFadeResponder : Responder
{
    [Header("References")]
    [SerializeField]
    private UITextWriter _textFade = default;

    [Space(5)]

    [Header("Settings")]
    [SerializeField]
    private UITextWriter.TypeWriterMethods _typeWriterMethods = default;
    [SerializeField]
    private UITextWriter.FadeMethods _fadeMethods = default;

    [Space(5)]

    [SerializeField, TextArea]
    private string _text = default;


    [Header("Fade Settings")]
    [SerializeField, Min(0.0f)]
    private float _fadeDelay = 0.0f;
    [SerializeField, Min(0.0f)]
    private float _fadeDuration = 0.0f;

    [Header("Type Writer Settings")]
    [SerializeField, Min(0.0f)]
    private float _startDelay = 0.0f;
    [SerializeField]
    private bool _typeFromLeftToRight = true;
    [SerializeField, Min(0.0f)]
    private float _delayBetweenLetters = 0.0f;
    [SerializeField, Min(0.0f)]
    private float _delayBetweenWords = 0.0f;

    public override void Respond(GameObject triggeringObject, ResponderState responderState)
    {
        _textFade.Fade(_fadeMethods, _fadeDelay, _fadeDuration, _text);

        _textFade.TypeWriter(_typeWriterMethods, _startDelay, _delayBetweenLetters, _delayBetweenWords, _typeFromLeftToRight, _text);
    }
}
