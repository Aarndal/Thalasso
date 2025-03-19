using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public class UITargetFrameRateSelection : MonoBehaviour
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

    private readonly SortedDictionary<string, int> _targetFrameRates = new();

    private void Awake()
    {
        if (!gameObject.TryGetComponent(out _tmpDropdown))
            _tmpDropdown = gameObject.AddComponent<TMP_Dropdown>();

        _tmpDropdown.captionText.color = _defaultCaptionTextColor;

        _vsyncToggle = _vsyncToggle != null ? _vsyncToggle : FindFirstObjectByType<UIVSyncToggle>(FindObjectsInactive.Include);

        if (_vsyncToggle == null)
        {
            Debug.LogErrorFormat("Missing GameObject with {0} component in scene!", _vsyncToggle);
        }

        SetupTargetFrameRateOptions();
    }

    private void OnEnable()
    {
        _tmpDropdown.onValueChanged.AddListener(SetTargetFrameRate);

        if (_vsyncToggle != null)
            _vsyncToggle.ValueChanged += OnVSyncChanged;
    }

    private void Start()
    {
        _tmpDropdown.value = GetTargetFrameRateIndex();
        _tmpDropdown.RefreshShownValue();

        if (_vsyncToggle != null)
            SetDropdownInteraction(_vsyncToggle.IsOn);
    }

    private void OnDisable()
    {
        if (_vsyncToggle != null)
            _vsyncToggle.ValueChanged -= OnVSyncChanged;

        _tmpDropdown.onValueChanged.RemoveListener(SetTargetFrameRate);
    }

    private int GetTargetFrameRateIndex()
    {
        return _tmpDropdown.options.FindIndex((optionData) => optionData.text == Screen.currentResolution.refreshRateRatio + " FPS");
    }

    private void OnVSyncChanged(uint id, bool isOn)
    {
        SetDropdownInteraction(isOn);
    }

    private void SetDropdownInteraction(bool isVsyncOn)
    {
        _tmpDropdown.interactable = !isVsyncOn;
        _tmpDropdown.captionText.color = isVsyncOn ? Color.grey : _defaultCaptionTextColor;
    }

    private void SetTargetFrameRate(int optionIndex)
    {
        Application.targetFrameRate = _targetFrameRates[_tmpDropdown.options[optionIndex].text];
    }

    private void SetupTargetFrameRateOptions()
    {
        _resolutions = Screen.resolutions;

        foreach (Resolution res in _resolutions)
        {
            _targetFrameRates.TryAdd(res.refreshRateRatio + " FPS", ((int)res.refreshRateRatio.value));
        }

        foreach (var frameRate in _supportedFrameRates)
        {
            _targetFrameRates.TryAdd(frameRate + " FPS", frameRate);
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
