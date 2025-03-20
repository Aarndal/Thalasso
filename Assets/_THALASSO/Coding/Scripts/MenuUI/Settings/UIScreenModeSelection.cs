using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Dropdown))]
public class UIScreenModeSelection : SettingElement<int>
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


    #region Data Management Methods
    public override void LoadData()
    {
        if (!PlayerPrefs.HasKey(SettingNames.ScreenMode))
        {
            PlayerPrefs.SetInt(SettingNames.ScreenMode, GetCurrentScreenModeIndex());
        }

        _tmpDropdown.value = PlayerPrefs.GetInt(SettingNames.ScreenMode);
        _tmpDropdown.RefreshShownValue();
    }

    protected override void SetData(int optionIndex)
    {
        Screen.fullScreenMode = _screenModes[_tmpDropdown.options[optionIndex].text];
    }

    public override void SaveData()
    {
        PlayerPrefs.SetInt(SettingNames.ScreenMode, GetCurrentScreenModeIndex());
    }

    public override void DeleteData()
    {
        if (PlayerPrefs.HasKey(SettingNames.ScreenMode))
            PlayerPrefs.DeleteKey(SettingNames.ScreenMode);

        LoadData();
    }
    #endregion


    #region Callback Functions
    protected override void AddListener()
    {
        base.AddListener();
        _tmpDropdown.onValueChanged.AddListener(SetData);
    }

    protected override void RemoveListener()
    {
        _tmpDropdown.onValueChanged.RemoveListener(SetData);
        base.RemoveListener();
    }
    #endregion


    private int GetCurrentScreenModeIndex()
    {
        string currentMode = _screenModes.Keys.First((mode) => _screenModes[mode] == CurrentScreenMode);

        return _tmpDropdown.options.FindIndex((optionData) => optionData.text == currentMode);
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