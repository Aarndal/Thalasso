public interface ILooseAir
{
    float MaxAirSupply { get; }
    float CurrentAirConsumptionRate { get; }

    void SetAirConsumptionRate(float newAirConsumptionRate);
    void SetAirConsumptionState(bool isConsumingAir);
}
