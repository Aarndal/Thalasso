using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Dropdown))]
public class UIScreenResolution : MonoBehaviour
{
    [SerializeField]
    private Sprite _sprite = default;
    [SerializeField]
    private Color _color = Color.white;

    private TMP_Dropdown _tmDropdown = default;

    private Resolution[] _screenResolutions;

    public readonly Dictionary<string, Resolution> AvailableResolutions = new();
    public FullScreenMode CurrentScreenMode => Screen.fullScreenMode;


    private void Awake()
    {
        if (!gameObject.TryGetComponent(out _tmDropdown))
            _tmDropdown = gameObject.AddComponent<TMP_Dropdown>();

        _screenResolutions = Screen.resolutions;

        for (int i = 0; i < _screenResolutions.Length; i++)
        {
            AvailableResolutions.TryAdd(_screenResolutions[i].width.ToString() + " x " + _screenResolutions[i].height.ToString(), _screenResolutions[i]);
        }
    }

    private void OnEnable()
    {
        _tmDropdown.onValueChanged.AddListener(SwitchScreenResolution);
    }

    private void Start()
    {
        _tmDropdown.options.Clear();

        for (int i = 0; i < AvailableResolutions.Count; i++)
        {
            _tmDropdown.value = i;
            _tmDropdown.options.Add(new TMP_Dropdown.OptionData(AvailableResolutions.ElementAt(i).Key, _sprite, _color));
        }
    }

    private void OnDisable()
    {
        _tmDropdown.onValueChanged.RemoveListener(SwitchScreenResolution);
    }

    private void SwitchScreenResolution(int optionNumber)
    {
        Screen.SetResolution(AvailableResolutions[_tmDropdown.options[optionNumber].text].width, AvailableResolutions[_tmDropdown.options[optionNumber].text].height, CurrentScreenMode);
    }
}