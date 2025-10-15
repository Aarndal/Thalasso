using Cysharp.Threading.Tasks;

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

        UniTask StartConsumingOxygen();
        bool TryStopConsumingOxygen();
    }
}
