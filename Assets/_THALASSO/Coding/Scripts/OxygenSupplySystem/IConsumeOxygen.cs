namespace OxygenSupplySystem
{
    /// <summary>
    /// Interface for components that consume oxygen.
    /// </summary>
    public interface IConsumeOxygen
    {
        float OxygenConsumptionRate { get; }
        float OxygenLevel { get; }
        float MaxOxygenLevel { get; }

        bool ConsumeOxygen();
        void SetOxygenConsumptionState(bool isConsumingOxygen);
    }
}
