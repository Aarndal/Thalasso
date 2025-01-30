using System;
using UnityEngine;

namespace ProgressionTracking
{
    [Serializable]
    public abstract class SolvableObjectBase : MonoBehaviour, IAmSolvable
    {
        [SerializeField]
        protected SO_SolvableData _solvableData;

        public bool IsSolved
        {
            get => _solvableData.IsSolved;
            protected set => _solvableData.IsSolved = value;
        }

        public abstract bool Solve();
    }
}
