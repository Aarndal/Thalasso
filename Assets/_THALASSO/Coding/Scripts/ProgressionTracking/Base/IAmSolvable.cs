namespace ProgressionTracking
{
    internal interface IAmSolvable
    {
        bool IsSolved { get; }

        bool Solve();
    }
}
