using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Dropdown))]
public class UITargetFrameRateSelection : SettingElement<int>
{
    [SerializeField]
    private UIVSyncToggle _vsyncToggle = default;

    [Space(5)]

    [SerializeField]
    private List<int> _supportedFrameRates = new();

    [Space(5)]

    [SerializeField]
    private Sprite _sprite = default;
    [SerializeField]
    private Color _color = Color.white;

    private TMP_Dropdown _tmpDropdown = default;
    private Color _defaultCaptionTextColor;

    private Resolution[] _resolutions;

    private readonly Dictionary<string, int> _targetFrameRates = new();

    public int CurrentFrameRate => ((int)Screen.currentResolution.refreshRateRatio.value);

    private void Awake()
    {
        if (!gameObject.TryGetComponent(out _tmpDropdown))
            _tmpDropdown = gameObject.AddComponent<TMP_Dropdown>();

        _defaultCaptionTextColor = _tmpDropdown.captionText.color;

        _vsyncToggle = _vsyncToggle != null ? _vsyncToggle : FindFirstObjectByType<UIVSyncToggle>(FindObjectsInactive.Include);

        if (_vsyncToggle == null)
        {
            Debug.LogErrorFormat("Missing GameObject with {0} component in scene!", _vsyncToggle);
        }

        SetupTargetFrameRateOptions();
    }


    #region Data Management Methods
    public override void LoadData()
    {
        if (!PlayerPrefs.HasKey(SettingNames.TargetFrameRate))
        {
            PlayerPrefs.SetInt(SettingNames.TargetFrameRate, GetTargetFrameRateIndex());
        }

        bool isVsyncOn = PlayerPrefs.GetInt(SettingNames.VSync) != 0;
        SetDropdownInteraction(isVsyncOn);

        _tmpDropdown.value = PlayerPrefs.GetInt(SettingNames.TargetFrameRate);
        _tmpDropdown.RefreshShownValue();
    }
    
    protected override void SetData(int optionIndex)
    {
        Application.targetFrameRate = _targetFrameRates[_tmpDropdown.options[optionIndex].text];
    }

    public override void SaveData()
    {
        PlayerPrefs.SetInt(SettingNames.TargetFrameRate, GetTargetFrameRateIndex());
    }

    public override void DeleteData()
    {
        if (PlayerPrefs.HasKey(SettingNames.TargetFrameRate))
            PlayerPrefs.DeleteKey(SettingNames.TargetFrameRate);

        LoadData();
    }
    #endregion


    #region Callback Functions
    protected override void AddListener()
    {
        base.AddListener();
        _tmpDropdown.onValueChanged.AddListener(SetData);

        if (_vsyncToggle != null)
            _vsyncToggle.ValueChanged += OnVSyncChanged;
    }

    protected override void RemoveListener()
    {
        if (_vsyncToggle != null)
            _vsyncToggle.ValueChanged -= OnVSyncChanged;

        _tmpDropdown.onValueChanged.RemoveListener(SetData);
        base.RemoveListener();
    }

    private void OnVSyncChanged(uint id, bool isOn) => SetDropdownInteraction(isOn);
    #endregion


    private int GetTargetFrameRateIndex()
    {
        return _tmpDropdown.options.FindIndex((optionData) => _targetFrameRates[optionData.text] == CurrentFrameRate);
    }

    private void SetDropdownInteraction(bool isVsyncOn)
    {
        _tmpDropdown.interactable = !isVsyncOn;
        _tmpDropdown.captionText.color = isVsyncOn ? Color.grey : _defaultCaptionTextColor;
    }

    private void SetupTargetFrameRateOptions()
    {
        foreach (var frameRate in _supportedFrameRates)
        {
            _targetFrameRates.TryAdd(frameRate + " FPS", frameRate);
        }

        _resolutions = Screen.resolutions;

        foreach (Resolution res in _resolutions)
        {
            _targetFrameRates.TryAdd(((int)res.refreshRateRatio.value) + " FPS", ((int)res.refreshRateRatio.value));
        }

        _targetFrameRates.TryAdd("Unlimited FPS", -1);
        /*
         * When QualitySettings.vSyncCount = 0 and Application.targetFrameRate = -1:
         *  Desktop: Content is rendered unsynchronized as fast as possible.
         *  Web: Content is rendered at the native display refresh rate.
         *  Android and iOS: Content is rendered at fixed 30 fps to conserve battery power, independent of the native refresh rate of the display.
         */

        _tmpDropdown.ClearOptions();

        foreach (var frameRateText in _targetFrameRates.Keys)
        {
            if (_sprite != null)
                _tmpDropdown.options.Add(new TMP_Dropdown.OptionData(frameRateText, _sprite, _color));
            else
                _tmpDropdown.options.Add(new TMP_Dropdown.OptionData(frameRateText));
        }
    }
}
