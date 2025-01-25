using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace ProgressionTracking
{
    [CreateAssetMenu(fileName = "NewProgressionTracker", menuName = "Scriptable Objects/Progression Tracker")]
    public class SO_ProgressionTracker : SO_Singleton
    {
        [SerializeField]
        private uint _id = 0;

        [SerializeField]
        private List<SO_UIntVariable> _solvableDependenciesID = new();

        [SerializeField]
        private bool _isCompleted = false;

        private readonly Dictionary<uint, bool> _progression = new();

        public uint ID => _id;
        public bool IsCompleted
        {
            get => _isCompleted;
            private set
            {
                if (_isCompleted != value)
                {
                    _isCompleted = value;

                    if (_isCompleted)
                        GlobalEventBus.Raise(GlobalEvents.Game.ProgressionCompleted, _id);
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();

            if (_solvableDependenciesID.Count > 0 && _solvableDependenciesID.Count != _progression.Count)
                foreach (var id in _solvableDependenciesID)
                    _progression.Add(id.Value, false);
        }

        private void OnEnable()
        {
            GlobalEventBus.Register(GlobalEvents.Game.HasBeenSolved, OnHasBeenSolved);
            IsCompleted = false;
        }

        private void OnDisable()
        {
            GlobalEventBus.Deregister(GlobalEvents.Game.HasBeenSolved, OnHasBeenSolved);
        }

        private void OnValidate()
        {
            if (_solvableDependenciesID.Count > 0 && _solvableDependenciesID.Count != _progression.Count)
            {
                _progression.Clear();
                foreach (var id in _solvableDependenciesID)
                    _progression.Add(id.Value, false);
            }
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
            if (IsCompleted = System.Linq.Enumerable.All(_progression, (o) => o.Value))
                Debug.Log("Progression of " + _id + " has been completed.");
        }
    }
}
