using UnityEngine;
using UnityEngine.UI;

public class ResumeGameButton : MonoBehaviour
{
    private Button _button = default;

    private void Awake()
    {
        _button = _button != null ? _button : GetComponentInChildren<Button>();

        if (_button == null)
            _button = gameObject.AddComponent<Button>();
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
        GlobalEventBus.Raise(GlobalEvents.Game.IsPaused, false);
    }
}
