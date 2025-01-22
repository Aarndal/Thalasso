using System.Collections.Generic;
using UnityEngine;

namespace ProgressionTracking
{
    public class ProgressionTracker : MonoBehaviour
    {
        [SerializeField]
        private uint _id = 0;

        [SerializeField]
        private HashSet<SolvableObjectBase> _solvableDependencies = new();

        private bool _isCompleted = false;

        public uint ID => _id;
        public bool IsCompleted
        {
            get => _isCompleted;
            private set
            {
                if (_isCompleted != value)
                    _isCompleted = value;

                if (_isCompleted)
                    GlobalEventBus.Raise(GlobalEvents.Game.ProgressionCompleted, _id);
            }
        }

        private void OnEnable()
        {
            foreach (var solvableDependency in _solvableDependencies)
                if (solvableDependency != null)
                    solvableDependency.HasBeenSolved += OnHasBeenSolved;
        }

        private void OnDisable()
        {
            foreach (var solvableDependency in _solvableDependencies)
                if (solvableDependency != null)
                    solvableDependency.HasBeenSolved -= OnHasBeenSolved;
        }

        private void OnHasBeenSolved()
        {
            foreach (var solvableDependency in _solvableDependencies)
            {
                if (!solvableDependency.IsSolved)
                    return;
            }

            IsCompleted = true;
        }
    }
}
