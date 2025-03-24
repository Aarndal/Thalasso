using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class UIVSyncToggle : SettingElement<bool>
{
    [SerializeField]
    private TMP_Text _tmpText = default;
    [SerializeField]
    private string _isOnText = "Active";
    [SerializeField]
    private Color _isOnColor = Color.black;
    [SerializeField]
    private string _isOffText = "Inactive";
    [SerializeField]
    private Color _isOffColor = Color.grey;
    [SerializeField]
    private SO_WwiseEvent _sound = default;

    private bool _isOn = true;
    private Toggle _toggle = default;
    private AkGameObj _akGameObject = default;

    public bool IsOn
    {
        get => _isOn;
        private set
        {
            if (value != _isOn)
            {
                _isOn = value;

                QualitySettings.vSyncCount = _isOn ? 1 : 0;
                /* 
                 * If vSyncCount == 1, rendering is synchronized to the vertical refresh rate of the display.
                 * If vSyncCount is set to 0, Unity does not synchronize rendering to vertical sync, and the field Application.targetFrameRate is instead used to pace the rendered frames.
                 */

                _tmpText.text = _isOn ? _isOnText : _isOffText;
                _tmpText.color = _isOn ? _isOnColor : _isOffColor;
                _toggle.targetGraphic.enabled = !_isOn;
                _sound.Play(_akGameObject);
                _valueChanged?.Invoke(ID, _isOn);
            }
        }
    }


    #region Unity Lifecycle Methods
    protected override void Awake()
    {
        MySettingsManager.Settings.TryAdd(transform.parent.name, this);

        if (!gameObject.TryGetComponent(out _toggle))
            _toggle = gameObject.AddComponent<Toggle>();

        _tmpText = _tmpText != null ? _tmpText : GetComponentInChildren<TMP_Text>();

        _akGameObject = _akGameObject != null ? _akGameObject : GetComponentInParent<AkGameObj>();
        if (_akGameObject == null)
            _akGameObject = gameObject.AddComponent<AkGameObj>();

        base.Awake();
    }

    private void OnEnable() => _toggle.onValueChanged.AddListener(SetData);

    private void OnDisable() => _toggle.onValueChanged.RemoveListener(SetData);
    #endregion


    #region Data Management Methods
    public override void LoadData()
    {
        if (!PlayerPrefs.HasKey(SettingNames.VSync))
        {
            PlayerPrefs.SetInt(SettingNames.VSync, 1); // 1 representing value: true | 0 representing value : false
        }

        _toggle.isOn = PlayerPrefs.GetInt(SettingNames.VSync) != 0;
    }

    protected override void SetData(bool isOn)
    {
        IsOn = isOn;

        int data = IsOn ? 1 : 0; // 1 representing value: true | 0 representing value : false
        PlayerPrefs.SetInt(SettingNames.VSync, data);
    }

    public override void DeleteData()
    {
        if (PlayerPrefs.HasKey(SettingNames.VSync))
            PlayerPrefs.DeleteKey(SettingNames.VSync);

        LoadData();
    }
    #endregion
}
