using System;
using UnityEngine;
using UnityEngine.UI;

[Obsolete]
public class WwiseButtonSound : MonoBehaviour
{
    [SerializeField]
    private AK.Wwise.Event _buttonSound = new();

    private Button _button = default;
    private AkGameObj _akGameObject = default;


    private void Awake()
    {
        _button = _button != null ? _button : GetComponentInChildren<Button>();

        if (_button == null)
            _button = gameObject.AddComponent<Button>();

        _akGameObject = _akGameObject != null ? _akGameObject : GetComponentInParent<AkGameObj>();

        if (_akGameObject == null)
            _akGameObject = gameObject.AddComponent<AkGameObj>();
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(OnClick);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(OnClick);
    }

    private void OnClick()
    {
        _buttonSound.Post(_akGameObject.gameObject);
    }
}
