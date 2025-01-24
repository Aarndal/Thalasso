using System.Collections.Generic;
using UnityEngine;

namespace ProgressionTracking
{
    [CreateAssetMenu(fileName = "NewProgressionTracker", menuName = "Scriptable Objects/Progression Tracker")]
    public class SO_ProgressionTracker : SO_Singleton
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
        
        protected override void Awake()
        {
            base.Awake();

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
            Debug.Log(this.name + " OnHasBeenSolved is being executed.");
            if (args[0] is uint id)
            {
                _progression[id] = true;
                Debug.Log(args[0] + " has been solved.");
                CheckProgression();
            }
        }

        private void CheckProgression()
        {
            IsCompleted = System.Linq.Enumerable.All(_progression, (o) => o.Value);
        }
    }
}
