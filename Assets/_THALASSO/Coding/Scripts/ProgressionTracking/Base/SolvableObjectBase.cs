using System;
using UnityEngine;

namespace ProgressionTracking
{
    [Serializable]
    public abstract class SolvableObjectBase : MonoBehaviour, IAmSolvable
    {
        public event Action HasBeenSolved;

        private bool _isSolved = false;

        public bool IsSolved
        {
            get => _isSolved;
            protected set
            {
                if (_isSolved != value)
                    _isSolved = value;

                if (_isSolved)
                    HasBeenSolved?.Invoke();
            }
        }

        public virtual void Solve() { }
    }
}
