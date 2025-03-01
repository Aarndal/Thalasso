using UnityEngine;

public class ButtonSounds : MonoBehaviour
{
    [SerializeField]
    private AK.Wwise.Event _buttonPress;
    [SerializeField]
    private AK.Wwise.Event _backButtonPress;

    //private AkGameObj _akGameObj;

    private void OnEnable()
    {
        //if (_akGameObj == null)
        //{
        //    _akGameObj = GetComponentInParent<AkGameObj>();
        //}
    }

    public void PlayButtonSound()
    {
        _buttonPress.Post(gameObject);
    }

    public void PlayBackButtonSound()
    {
        _backButtonPress.Post(gameObject);
    }
}
