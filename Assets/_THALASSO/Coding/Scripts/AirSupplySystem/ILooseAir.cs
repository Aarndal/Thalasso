public interface ILooseAir
{
    float MaxAirSupply { get; }
    float CurrentAirLossRate { get; }

    void ChangeAirLossRate(float newAirLossRate);
    void LooseAir(bool looseAir);
}
