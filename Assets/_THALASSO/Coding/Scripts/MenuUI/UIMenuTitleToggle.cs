using TMPro;
using UnityEngine;

public class UIMenuTitleToggle : MonoBehaviour
{
    [SerializeField]
    private Canvas _canvas = default;

    private TMP_Text _title = default;
    
    private void Awake()
    {
        _title = _title != null ? _title : GetComponentInParent<TMP_Text>();

        if (_canvas == null)
        {
            Debug.LogErrorFormat("<color=red>No Canvas assigned</color> on <color=cyan>{0}'s</color> {1} component!", gameObject.name, this);
        }
    }

    private void OnEnable()
    {
        GlobalEventBus.Register(GlobalEvents.UI.CanvasEnabled, OnCanvasEnabled);
        GlobalEventBus.Register(GlobalEvents.UI.CanvasDisabled, OnCanvasDisabled);
    }

    private void Start()
    {
        SetSubtitleState(_canvas.enabled);
    }

    private void OnDisable()
    {
        GlobalEventBus.Deregister(GlobalEvents.UI.CanvasDisabled, OnCanvasDisabled);
        GlobalEventBus.Deregister(GlobalEvents.UI.CanvasEnabled, OnCanvasEnabled);
    }

    private void OnCanvasDisabled(object[] eventArgs)
    {
        foreach (var arg in eventArgs)
        {
            if (arg is string canvasName)
            {
                if (canvasName == _canvas.name)
                    SetSubtitleState(false);
            }
        }
    }

    private void OnCanvasEnabled(object[] eventArgs)
    {
        foreach (var arg in eventArgs)
        {
            if (arg is string canvasName)
            {
                if (canvasName == _canvas.name)
                    SetSubtitleState(true);
            }
        }
    }

    private void SetSubtitleState(bool enable)
    {
        if(_canvas.enabled == enable && _title != null)
        {
            _title.enabled = enable;
        }
    }
}
