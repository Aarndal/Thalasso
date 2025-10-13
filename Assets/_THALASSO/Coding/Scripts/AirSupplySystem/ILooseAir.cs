public interface ILooseAir
{
    float MaxOxygenLevel { get; }
    float ActiveConsumptionRate { get; }
    //float CurrentAirSupply { get; }

    void SetAirConsumptionRate(float newAirConsumptionRate);
    void SetOxygenConsumptionState(bool isConsumingAir);
}
