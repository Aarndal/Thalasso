using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class UIVSyncToggle : MonoBehaviour, INotifyValueChanged<bool>
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

    private Toggle _toggle = default;

    public uint ID => ((uint)GetInstanceID());
    public bool IsOn => _toggle.isOn;

    private Action<uint, bool> _valueChanged;

    public event Action<uint, bool> ValueChanged
    {
        add
        {
            _valueChanged -= value;
            _valueChanged += value;
        }
        remove => _valueChanged -= value;
    }

    private void Awake()
    {
        if (!gameObject.TryGetComponent(out _toggle))
            _toggle = gameObject.AddComponent<Toggle>();

        _tmpText = _tmpText != null ? _tmpText : GetComponentInChildren<TMP_Text>();
    }

    private void OnEnable()
    {
        _toggle.onValueChanged.AddListener(OnVSyncChanged);
    }

    private void Start()
    {
        _toggle.isOn = true;
        OnVSyncChanged(true);
    }

    private void OnDisable()
    {
        _toggle.onValueChanged.RemoveListener(OnVSyncChanged);
    }

    private void OnVSyncChanged(bool isOn)
    {
        QualitySettings.vSyncCount = isOn ? 1 : 0;
        /* If vSyncCount == 1, rendering is synchronized to the vertical refresh rate of the display.
         * If vSyncCount is set to 0, Unity does not synchronize rendering to vertical sync, and the field Application.targetFrameRate is instead used to pace the rendered frames.*/

        _tmpText.text = isOn ? _isOnText : _isOffText;
        _tmpText.color = isOn ? _isOnColor : _isOffColor;
        _toggle.targetGraphic.enabled = !isOn;

        _valueChanged?.Invoke(ID, isOn);
    }
}
