namespace ProgressionTracking
{
    public interface IAmSolvable
    {
        bool IsSolved { get; }

        bool Solve();
    }
}
