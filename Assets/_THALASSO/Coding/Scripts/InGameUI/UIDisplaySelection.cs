using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Dropdown))]
public class UIDisplaySelection : MonoBehaviour
{
    [SerializeField]
    private Sprite _sprite = default;
    [SerializeField]
    private Color _color = Color.white;

    private TMP_Dropdown _tmpDropdown = default;

    private readonly List<DisplayInfo> _displayLayout = new();

    public DisplayInfo CurrentDisplay => Screen.mainWindowDisplayInfo;

    private void Awake()
    {
        if (!gameObject.TryGetComponent(out _tmpDropdown))
            _tmpDropdown = gameObject.AddComponent<TMP_Dropdown>();

        SetupDisplayOptions();
    }

    private void OnEnable()
    {
        _tmpDropdown.onValueChanged.AddListener(SetDisplay);
    }

    private void Start()
    {
        _tmpDropdown.value = GetCurrentDisplayIndex();
        _tmpDropdown.RefreshShownValue();
    }

    private void OnDisable()
    {
        _tmpDropdown.onValueChanged.RemoveListener(SetDisplay);
    }

    private int GetCurrentDisplayIndex()
    {
        return _tmpDropdown.options.FindIndex((optionData) => optionData.text == CurrentDisplay.name);
    }

    private void SetDisplay(int optionIndex)
    {
        DisplayInfo newDisplay = _displayLayout.Find((display) => _tmpDropdown.options[optionIndex].text == display.name);
        Screen.MoveMainWindowTo(newDisplay, new Vector2Int(0,0));
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
