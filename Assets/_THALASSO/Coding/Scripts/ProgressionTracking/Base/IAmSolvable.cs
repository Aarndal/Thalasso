using System;

namespace ProgressionTracking
{
    internal interface IAmSolvable
    {
        public event Action HasBeenSolved;

        public bool IsSolved { get; }

        void Solve();
    }
}
