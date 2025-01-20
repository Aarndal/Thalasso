using System.Collections.Generic;
using UnityEngine;

namespace ProgressionTracking
{
    [CreateAssetMenu(fileName = "NewProgessionTracker", menuName = "Scriptable Objects/ProgessionTracker")]
    public class SO_ProgessionTracker : ScriptableObject
    {
        [SerializeField]
        private uint _id = 0;

        [SerializeField]
        private List<IAmSolvable> _solvableDependencies = new List<IAmSolvable>();

        private bool _isCompleted = false;

        public uint ID { get => _id; private set => _id = value; }
        public bool IsCompleted { get => _isCompleted; private set => _isCompleted = value; }

        private void Awake()
        {
            IsCompleted = false;
        }

        private void OnEnable()
        {
            foreach (var solvableDependency in _solvableDependencies)
                solvableDependency.HasBeenSolved += OnHasBeenSolved;
        }

        private void OnDisable()
        {
            foreach (var solvableDependency in _solvableDependencies)
                solvableDependency.HasBeenSolved -= OnHasBeenSolved;
        }

        private void OnHasBeenSolved()
        {
            foreach (var solvableDependency in _solvableDependencies)
            {
                if (!solvableDependency.IsSolved)
                    return;

                IsCompleted = true;
                GlobalEventBus.Raise(GlobalEvents.Game.ProgessionCompleted, _id);
            }
        }
    }
}
