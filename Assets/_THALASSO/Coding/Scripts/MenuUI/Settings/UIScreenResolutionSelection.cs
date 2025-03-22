using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Dropdown))]
public class UIScreenResolutionSelection : SettingElement<int>
{
    [SerializeField]
    private Sprite _sprite = default;
    [SerializeField]
    private Color _color = Color.white;

    private TMP_Dropdown _tmpDropdown = default;

    private Resolution[] _screenResolutions;

    public readonly Dictionary<string, Resolution> AvailableResolutions = new();
    public FullScreenMode CurrentScreenMode => Screen.fullScreenMode;
    public Resolution CurrentResolution => Screen.currentResolution;
    public RefreshRate CurrentRefreshRate => Screen.currentResolution.refreshRateRatio;


    #region Unity Lifecycle Methods
    protected override void Awake()
    {
        MySettingsManager.Settings.TryAdd(transform.parent.name, this);

        if (!gameObject.TryGetComponent(out _tmpDropdown))
            _tmpDropdown = gameObject.AddComponent<TMP_Dropdown>();

        SetupResolutionOptions();

        base.Awake();
    }

    private void OnEnable() => _tmpDropdown.onValueChanged.AddListener(SetData);

    private void OnDisable() => _tmpDropdown.onValueChanged.RemoveListener(SetData);
    #endregion


    #region Data Management Methods
    public override void LoadData()
    {
        if (!PlayerPrefs.HasKey(SettingNames.ScreenResolution))
        {
            PlayerPrefs.SetInt(SettingNames.ScreenResolution, GetCurrentResolutionIndex());
        }

        _tmpDropdown.value = PlayerPrefs.GetInt(SettingNames.ScreenResolution);
        _tmpDropdown.RefreshShownValue();
    }

    protected override void SetData(int optionIndex)
    {
        Screen.SetResolution(AvailableResolutions[_tmpDropdown.options[optionIndex].text].width, AvailableResolutions[_tmpDropdown.options[optionIndex].text].height, CurrentScreenMode);
        PlayerPrefs.SetInt(SettingNames.ScreenResolution, GetCurrentResolutionIndex());
    }

    public override void DeleteData()
    {
        if (PlayerPrefs.HasKey(SettingNames.ScreenResolution))
            PlayerPrefs.DeleteKey(SettingNames.ScreenResolution);

        LoadData();
    }
    #endregion


    private int GetCurrentResolutionIndex()
    {
        string currentRes = AvailableResolutions.Keys.First((res) => AvailableResolutions[res].width == CurrentResolution.width && AvailableResolutions[res].height == CurrentResolution.height);

        return _tmpDropdown.options.FindIndex((optionData) => optionData.text == currentRes);
    }

    public void SetupResolutionOptions()
    {
        _screenResolutions = Screen.resolutions;

        foreach (var res in _screenResolutions)
        {
            if (res.refreshRateRatio.numerator == CurrentRefreshRate.numerator)
                AvailableResolutions.TryAdd(res.width + " x " + res.height, res);
        }

        _tmpDropdown.ClearOptions();

        foreach (var text in AvailableResolutions.Keys)
        {
            if (_sprite != null)
                _tmpDropdown.options.Add(new TMP_Dropdown.OptionData(text, _sprite, _color));
            else
                _tmpDropdown.options.Add(new TMP_Dropdown.OptionData(text));
        }
    }
}