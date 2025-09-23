using UnityEngine;

public class AirSupply : MonoBehaviour, ILooseAir
{
    [SerializeField]
    private SOAirSupplyData _airSupplyData = null;
    [SerializeField]
    private bool _looseAir = true;

    public float MaxAirSupply => _airSupplyData.MaxAirSupply;
    public float CurrentAirLossRate => _airSupplyData.CurrentAirLossRate;

    private void Awake()
    {
        if (_airSupplyData == null)
        {
#if UNITY_EDITOR
            Debug.LogErrorFormat("AirSupplyData is not assigned for {0}", gameObject.name);
#endif
        }

        ResetAirSupply();
    }

    private void OnEnable()
    {
        _airSupplyData.AirSupplyDepleted += OnAirSupplyDepleted;
    }

    private void Update()
    {
        LooseAir(_looseAir);
    }

    private void OnAirSupplyDepleted()
    {
        _looseAir = false;
    }

    public void ChangeAirLossRate(float newAirLossRate)
    {
        //TODO: Add logic
    }

    public void ResetAirSupply()
    {
        _airSupplyData.CurrentAirSupply = _airSupplyData.MaxAirSupply;
        _airSupplyData.CurrentAirLossRate = 0f; // Start with no air loss
    }

    public void LooseAir(bool looseAir = true)
    {
        if (!looseAir) return;

        _airSupplyData.CurrentAirSupply -= CurrentAirLossRate * Time.deltaTime;
    }
}
