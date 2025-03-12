using System;
using UnityEngine;

[Obsolete]
public class ButtonSounds : MonoBehaviour
{
#if WWISE_2024_OR_LATER
    [SerializeField]
    private AK.Wwise.Event _buttonPress;
    [SerializeField]
    private AK.Wwise.Event _backButtonPress;

    private AkGameObj _akGameObject;

    private void Awake()
    {
        _akGameObject = _akGameObject != null ? _akGameObject : GetComponentInParent<AkGameObj>();

        if (_akGameObject == null)
            _akGameObject = gameObject.AddComponent<AkGameObj>();
    }

    public void PlayButtonSound()
    {
        _buttonPress.Post(_akGameObject.gameObject);
    }

    public void PlayBackButtonSound()
    {
        _backButtonPress.Post(_akGameObject.gameObject);
    }
#endif
}
