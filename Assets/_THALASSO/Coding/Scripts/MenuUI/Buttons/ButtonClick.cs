using UnityEngine;
using UnityEngine.UI;

public abstract class ButtonClick : MonoBehaviour
{
    [SerializeField]
    protected SO_WwiseEvent[] _buttonSounds;

    protected Button _button = default;
    protected AkGameObj _akGameObject = default;

    protected virtual void Awake()
    {
        _button = _button != null ? _button : GetComponentInChildren<Button>();

        if (_button == null)
            _button = gameObject.AddComponent<Button>();

        _akGameObject = _akGameObject != null ? _akGameObject : GetComponentInParent<AkGameObj>();

        if (_akGameObject == null)
            _akGameObject = gameObject.AddComponent<AkGameObj>();
    }

    protected virtual void OnEnable()
    {
        _button.onClick.AddListener(OnClicked);
    }

    protected virtual void OnDisable()
    {
        _button.onClick.RemoveListener(OnClicked);
    }

    protected virtual void OnClicked()
    {
        if (_buttonSounds.Length > 0)
        {
            foreach (var sound in _buttonSounds)
            {
                sound.Post(_akGameObject);
            }
        }
    }
}