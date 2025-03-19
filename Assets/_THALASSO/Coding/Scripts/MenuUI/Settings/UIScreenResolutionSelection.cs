using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Dropdown))]
public class UIScreenResolutionSelection : MonoBehaviour
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

    private void Awake()
    {
        if (!gameObject.TryGetComponent(out _tmpDropdown))
            _tmpDropdown = gameObject.AddComponent<TMP_Dropdown>();

        SetupResolutionOptions();
    }

    private void OnEnable()
    {
        _tmpDropdown.onValueChanged.AddListener(SetScreenResolution);
    }

    private void Start()
    {
        RefreshCurrentResolutionSelection();
    }

    private void OnDisable()
    {
        _tmpDropdown.onValueChanged.RemoveListener(SetScreenResolution);
    }

    public void RefreshCurrentResolutionSelection()
    {
        _tmpDropdown.value = GetCurrentResolutionIndex();
        _tmpDropdown.RefreshShownValue();
    }

    public void SetupResolutionOptions()
    {
        _screenResolutions = Screen.resolutions;

        foreach (var res in _screenResolutions)
        {
            if (res.refreshRateRatio.numerator == CurrentRefreshRate.numerator)
                AvailableResolutions.TryAdd(res.width + " x " + res.height + " : " + res.refreshRateRatio + " Hz", res);
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

    private int GetCurrentResolutionIndex()
    {
        string currentRes = AvailableResolutions.Keys.First((res) => AvailableResolutions[res].width == CurrentResolution.width && AvailableResolutions[res].height == CurrentResolution.height);

        return _tmpDropdown.options.FindIndex((optionData) => optionData.text == currentRes);
    }

    private void SetScreenResolution(int optionIndex)
    {
        Screen.SetResolution(AvailableResolutions[_tmpDropdown.options[optionIndex].text].width, AvailableResolutions[_tmpDropdown.options[optionIndex].text].height, CurrentScreenMode);
    }
}