using Cysharp.Threading.Tasks;
using System.Threading;

namespace OxygenSupplySystem
{
    /// <summary>
    /// Interface for components that consume oxygen.
    /// </summary>
    public interface IConsumeOxygen
    {
        float OxygenConsumptionRate { get; }
        float OxygenLevel { get; set; }
        float MaxOxygenLevel { get; }

        UniTask<bool> TryStartConsumingOxygenAsync(CancellationToken cancellationToken = default);
        bool TryStopConsumingOxygen();
    }
}
