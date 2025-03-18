using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Dropdown))]
public class UIScreenMode : MonoBehaviour
{
    [SerializeField]
    private Sprite _sprite = default;
    [SerializeField]
    private Color _color = Color.white;

    private TMP_Dropdown _tmDropdown = default;

    private Dictionary<string, FullScreenMode> _screenModes = new();

    public FullScreenMode CurrentScreenMode => Screen.fullScreenMode;

    private void Awake()
    {
        if (!gameObject.TryGetComponent(out _tmDropdown))
            _tmDropdown = gameObject.AddComponent<TMP_Dropdown>();

        _screenModes.TryAdd("Full Screen", FullScreenMode.FullScreenWindow);
        _screenModes.TryAdd("Borderless Window", FullScreenMode.MaximizedWindow);
        _screenModes.TryAdd("Windowed", FullScreenMode.Windowed);
    }

    private void OnEnable()
    {
        _tmDropdown.onValueChanged.AddListener(SwitchScreenMode);
    }

    private void Start()
    {
        _tmDropdown.options.Clear();

        for (int i = 0; i < _screenModes.Count; i++)
        {
            _tmDropdown.value = i;
            _tmDropdown.options.Add(new TMP_Dropdown.OptionData(_screenModes.ElementAt(i).Key, _sprite, _color));
        }
    }

    private void OnDisable()
    {
        _tmDropdown.onValueChanged.RemoveListener(SwitchScreenMode);
    }

    private void SwitchScreenMode(int optionNumber)
    {
        Screen.fullScreenMode = _screenModes[_tmDropdown.options[optionNumber].text];
    }
}