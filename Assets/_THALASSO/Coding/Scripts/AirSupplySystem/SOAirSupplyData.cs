using System;
using UnityEngine;

[CreateAssetMenu(fileName = "newAirSupplyData", menuName = "Scriptable Objects/Air Supply Data", order = 0)]
public class SOAirSupplyData : ScriptableObject
{
    [SerializeField]
    private float _currentAirSupply = 100f;
    [SerializeField]
    private float _maxAirSupply = 100f;

    [Header("Air Loss Rate Data")]
    [SerializeField]
    private float _currentAirLossRate = 0f;
    [SerializeField]
    private float _standardAirLossRate = 0.05f;
    [SerializeField, Range(1f, 10f)]
    private float _maxAirLossRate = 5f;
    [SerializeField]
    private float _minAirLossRate = 0f;

    public float MaxAirSupply => _maxAirSupply;
    public float StandardAirLossRate => _standardAirLossRate;
    public float MaxAirLossRate => _maxAirLossRate;
    public float MinAirLossRate => _minAirLossRate;

    public event Action<float> AirSupplyChanged;
    public event Action AirSupplyDepleted;

    public float CurrentAirSupply
    {
        get => _currentAirSupply;
        set
        {
            if (_currentAirSupply != value)
            {
                _currentAirSupply = Mathf.Clamp(value, 0f, MaxAirSupply);
                AirSupplyChanged?.Invoke(_currentAirSupply);

                if (_currentAirSupply <= 0f)
                {
#if UNITY_EDITOR
                    Debug.LogWarning("Air supply has been depleted!");
#endif
                    AirSupplyDepleted?.Invoke();
                }
            }
        }
    }

    public float CurrentAirLossRate
    {
        get => _currentAirLossRate;
        set => _currentAirLossRate = Mathf.Clamp(value, MinAirLossRate, MaxAirLossRate);
    }
}
