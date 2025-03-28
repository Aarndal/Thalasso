﻿using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Dropdown))]
public class UIDisplaySelection : SettingElement<int>
{
    [SerializeField]
    private Sprite _sprite = default;
    [SerializeField]
    private Color _color = Color.white;

    private TMP_Dropdown _tmpDropdown = default;

    private readonly List<DisplayInfo> _displayLayout = new();

    public DisplayInfo CurrentDisplay => Screen.mainWindowDisplayInfo;


    #region Unity Lifecycle Methods
    protected override void Awake()
    {
        MySettingsManager.Settings.TryAdd(transform.parent.name, this);

        if (!gameObject.TryGetComponent(out _tmpDropdown))
            _tmpDropdown = gameObject.AddComponent<TMP_Dropdown>();

        SetupDisplayOptions();

        base.Awake();
    }

    private void OnEnable() => _tmpDropdown.onValueChanged.AddListener(SetData);

    private void OnDisable() => _tmpDropdown.onValueChanged.RemoveListener(SetData);
    #endregion


    #region Data Management Methods
    public override void LoadData()
    {
        if (!PlayerPrefs.HasKey(SettingNames.Display))
        {
            PlayerPrefs.SetInt(SettingNames.Display, GetCurrentDisplayIndex());
        }

        _tmpDropdown.value = PlayerPrefs.GetInt(SettingNames.Display);
        _tmpDropdown.RefreshShownValue();
    }

    protected override void SetData(int optionIndex)
    {
        DisplayInfo newDisplay = _displayLayout.Find((display) => _tmpDropdown.options[optionIndex].text == display.name);
        Screen.MoveMainWindowTo(newDisplay, new Vector2Int(0, 0));
        PlayerPrefs.SetInt(SettingNames.Display, GetCurrentDisplayIndex());
    }

    public override void DeleteData()
    {
        if (PlayerPrefs.HasKey(SettingNames.Display))
            PlayerPrefs.DeleteKey(SettingNames.Display);

        LoadData();
    }
    #endregion


    private int GetCurrentDisplayIndex()
    {
        return _tmpDropdown.options.FindIndex((optionData) => optionData.text == CurrentDisplay.name);
    }

    private void SetupDisplayOptions()
    {
        Screen.GetDisplayLayout(_displayLayout);

        _tmpDropdown.ClearOptions();

        foreach (var display in _displayLayout)
        {
            if (_sprite != null)
                _tmpDropdown.options.Add(new TMP_Dropdown.OptionData(display.name, _sprite, _color));
            else
                _tmpDropdown.options.Add(new TMP_Dropdown.OptionData(display.name));
        }
    }
}
