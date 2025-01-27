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
        private List<SO_UIntVariable> _solvableDependenciesID = new();

        [SerializeField]
        private bool _isCompleted = false;

        private readonly Dictionary<uint, bool> _progression = new();

        public uint ID => _id;
        public bool IsCompleted
        {
            get => _isCompleted;
            private set => _isCompleted = value;
        }

        protected override void Awake()
        {
            base.Awake();

            if (_solvableDependenciesID.Count > 0 && _solvableDependenciesID.Count != _progression.Count)
                InitializeProgression();
        }

        private void OnEnable()
        {
            GlobalEventBus.Register(GlobalEvents.Game.HasBeenSolved, OnHasBeenSolved);
        }

        private void OnDisable()
        {
            GlobalEventBus.Deregister(GlobalEvents.Game.HasBeenSolved, OnHasBeenSolved);

#if UNITY_EDITOR
            if (IsCompleted)
                IsCompleted = false;
#endif
        }

        private void OnValidate()
        {
            if (_solvableDependenciesID.Count > 0 && _solvableDependenciesID.Count != _progression.Count)
                InitializeProgression();
        }

        private void OnHasBeenSolved(object[] args)
        {
            if (args[0] is uint id && _progression.ContainsKey(id))
            {
                _progression[id] = true;
                CheckProgression();
            }
        }

        private void CheckProgression()
        {
            string debugColor = "cyan";

            Debug.LogFormat("<color={0}>Progression Tracker ID: {1}</color>", debugColor, _id);
            foreach (var progress in _progression)
            {
                debugColor = progress.Value ? "green" : "red";
                Debug.LogFormat("Solvable ID: <color={0}>{1}</color> | Is Solved: <color={0}>{2}</color>", debugColor, progress.Key, progress.Value);
            }

            if (IsCompleted = System.Linq.Enumerable.All(_progression, (o) => o.Value))
            {
                GlobalEventBus.Raise(GlobalEvents.Game.ProgressionCompleted, _id);
                Debug.LogFormat("<color={0}>Progression of {1} has been completed</color>", debugColor, _id);
            }
        }

        private void InitializeProgression()
        {
            _progression.Clear();
            foreach (var id in _solvableDependenciesID)
                _progression.Add(id.Value, false);
        }
    }
}
