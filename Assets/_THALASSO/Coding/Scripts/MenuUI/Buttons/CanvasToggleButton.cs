using UnityEngine;

public class CanvasToggleButton : ButtonClick
{
    [SerializeField]
    private bool _toggleOtherCanvas = true;
    [SerializeField]
    private Canvas _targetCanvas = default;

    private Canvas _currentCanvas = default;

    protected override void Awake()
    {
        base.Awake();

        _currentCanvas = _currentCanvas != null ? _currentCanvas : GetComponentInParent<Canvas>();

        if (_toggleOtherCanvas && _targetCanvas == null)
        {
            Debug.LogErrorFormat("<color=red>No Target Canvas assigned</color> on <color=cyan>{0}'s</color> {1} component!", gameObject.name, this);
        }
    }


    protected override void OnClicked()
    {
        base.OnClicked();

        if (_currentCanvas.enabled)
        {
            _currentCanvas.enabled = !_currentCanvas.enabled;
            GlobalEventBus.Raise(GlobalEvents.UI.CanvasDisabled, _currentCanvas.name);
        }

        if (_toggleOtherCanvas && _targetCanvas != null && !_targetCanvas.enabled)
        {
            _targetCanvas.enabled = !_targetCanvas.enabled;
            GlobalEventBus.Raise(GlobalEvents.UI.CanvasEnabled, _targetCanvas.name);
        }
    }
}
