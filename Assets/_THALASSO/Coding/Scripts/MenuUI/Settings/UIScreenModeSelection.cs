using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Dropdown))]
public class UIScreenModeSelection : MonoBehaviour
{
    [SerializeField]
    private Sprite _sprite = default;
    [SerializeField]
    private Color _color = Color.white;

    private TMP_Dropdown _tmpDropdown = default;

    private readonly Dictionary<string, FullScreenMode> _screenModes = new();

    public FullScreenMode CurrentScreenMode => Screen.fullScreenMode;

    private void Awake()
    {
        if (!gameObject.TryGetComponent(out _tmpDropdown))
            _tmpDropdown = gameObject.AddComponent<TMP_Dropdown>();

        SetupScreenModeOptions();
    }

    private void OnEnable()
    {
        _tmpDropdown.onValueChanged.AddListener(SetScreenMode);
    }

    private void Start()
    {
        _tmpDropdown.value = GetCurrentScreenModeIndex();
        _tmpDropdown.RefreshShownValue();
    }


    private void OnDisable()
    {
        _tmpDropdown.onValueChanged.RemoveListener(SetScreenMode);
    }

    private int GetCurrentScreenModeIndex()
    {
        string currentMode = _screenModes.Keys.First((mode) => _screenModes[mode] == CurrentScreenMode);

        return _tmpDropdown.options.FindIndex((optionData) => optionData.text == currentMode);
    }

    private void SetScreenMode(int optionIndex)
    {
        Screen.fullScreenMode = _screenModes[_tmpDropdown.options[optionIndex].text];
    }

    private void SetupScreenModeOptions()
    {
        _screenModes.TryAdd("Full Screen", FullScreenMode.ExclusiveFullScreen);
        _screenModes.TryAdd("Borderless Window", FullScreenMode.FullScreenWindow);
        _screenModes.TryAdd("Windowed", FullScreenMode.Windowed);

        _tmpDropdown.ClearOptions();

        foreach (var modeName in _screenModes.Keys)
        {
            if (_sprite != null)
                _tmpDropdown.options.Add(new TMP_Dropdown.OptionData(modeName, _sprite, _color));
            else
                _tmpDropdown.options.Add(new TMP_Dropdown.OptionData(modeName));
        }
    }
}