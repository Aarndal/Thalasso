using UnityEngine;
using UnityEngine.UI;

public class CanvasSwitchButton : MonoBehaviour
{
    [SerializeField]
    private bool _switchToOtherCanvas = true;
    [SerializeField]
    private Canvas _targetCanvas = default;

    private Canvas _currentCanvas = default;
    private Button _button = default;

    private void Awake()
    {
        _button = _button != null ? _button : GetComponentInChildren<Button>();

        if (_button == null)
            _button = gameObject.AddComponent<Button>();

        _currentCanvas = _currentCanvas != null ? _currentCanvas : GetComponentInParent<Canvas>();

        if (_switchToOtherCanvas && _targetCanvas == null)
        {
            Debug.LogErrorFormat("<color=red>No Target Canvas assigned</color> on <color=cyan>{0}'s</color> {1} component!", gameObject.name, this);
        }
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(OnClicked);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(OnClicked);
    }

    private void OnClicked()
    {
        _currentCanvas.enabled = !_currentCanvas.enabled;

        if (!_targetCanvas.isActiveAndEnabled && _switchToOtherCanvas)
        {
            _targetCanvas.enabled = !_targetCanvas.enabled;
        }
    }
}
