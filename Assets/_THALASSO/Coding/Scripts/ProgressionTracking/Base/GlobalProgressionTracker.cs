using System.Collections.Generic;
using UnityEngine;

namespace ProgressionTracking
{
    public class GlobalProgressionTracker : MonoBehaviour
    {
        [SerializeField]
        private uint _id = 0;

        [SerializeField]
        private List<uint> _solvableDependenciesID = new();

        private bool _isCompleted = false;
        private Dictionary<uint, bool> _progression = new();

        public uint ID => _id;
        public bool IsCompleted
        {
            get => _isCompleted;
            private set
            {
                if (_isCompleted != value && value == true)
                {
                    _isCompleted = value;
                    GlobalEventBus.Raise(GlobalEvents.Game.ProgressionCompleted, _id);
                }
            }
        }

        private void Awake()
        {
            if (_solvableDependenciesID.Count > 0)
                foreach (var id in _solvableDependenciesID)
                    _progression.Add(id, false);
        }

        private void OnEnable()
        {
            foreach (var id in _progression)
                GlobalEventBus.Register(GlobalEvents.Game.HasBeenSolved, OnHasBeenSolved);
        }

        private void OnDisable()
        {
            foreach (var id in _progression)
                GlobalEventBus.Deregister(GlobalEvents.Game.HasBeenSolved, OnHasBeenSolved);
        }

        private void OnHasBeenSolved(object[] args)
        {
            if (args[0] is uint id)
            {
                _progression[id] = true;

                CheckProgression();
            }
        }

        private void CheckProgression()
        {
            IsCompleted = System.Linq.Enumerable.All(_progression, (o) => o.Value);
        }
    }
}
