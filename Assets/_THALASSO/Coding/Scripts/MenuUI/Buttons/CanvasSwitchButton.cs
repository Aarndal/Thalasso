using UnityEngine;

public class CanvasSwitchButton : ButtonClick
{
    [SerializeField]
    private bool _switchToOtherCanvas = true;
    [SerializeField]
    private Canvas _targetCanvas = default;

    private Canvas _currentCanvas = default;

    protected override void Awake()
    {
        base.Awake();

        _currentCanvas = _currentCanvas != null ? _currentCanvas : GetComponentInParent<Canvas>();

        if (_switchToOtherCanvas && _targetCanvas == null)
        {
            Debug.LogErrorFormat("<color=red>No Target Canvas assigned</color> on <color=cyan>{0}'s</color> {1} component!", gameObject.name, this);
        }
    }

    protected override void OnClicked()
    {
        base.OnClicked();

        _currentCanvas.enabled = !_currentCanvas.enabled;

        if (!_targetCanvas.enabled && _switchToOtherCanvas)
        {
            _targetCanvas.enabled = !_targetCanvas.enabled;
        }
    }
}
