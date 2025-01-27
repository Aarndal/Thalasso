using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProgressionTracking
{
    [CreateAssetMenu(fileName = "NewProgressionTracker02", menuName = "Scriptable Objects/Progression Tracker02")]
    public class SO_ProgressionTracker02 : SO_ResettableDataSingleton
    {
        [SerializeField]
        private uint _id = 0;

        [SerializeField]
        private bool _isCompleted = false;

        [SerializeField]
        private List<SO_SolvableObject> _solvableDependenciesID = new();

        private readonly Dictionary<uint, SO_SolvableObject> _progression = new();

        public uint ID => _id;
        public bool IsCompleted { get => _isCompleted; private set => _isCompleted = value; }

        protected override void Awake()
        {
            base.Awake();

            if (_solvableDependenciesID.Count > 0 && _solvableDependenciesID.Count != _progression.Count)
                InitializeProgression();

            CheckProgression();
        }


        protected override void OnEnable()
        {
            base.OnEnable();

            foreach (var solvableObject in _progression.Values)
                solvableObject.ValueChanged += OnSolveStateChanged;
        }


        protected override void OnDisable()
        {
            foreach (var solvableObject in _progression.Values)
                solvableObject.ValueChanged -= OnSolveStateChanged;

            base.OnDisable();
        }

        private void OnValidate()
        {
            if (_solvableDependenciesID.Count > 0 && _solvableDependenciesID.Count != _progression.Count)
                InitializeProgression();
        }

        private void OnSolveStateChanged(uint id, bool isSolved)
        {
            if (_progression.ContainsKey(id))
                CheckProgression();
        }

        public override void ResetData() => IsCompleted = false;

        private void CheckProgression()
        {
            string debugColor = "cyan";

            Debug.LogFormat("<color={0}>Progression Tracker ID: {1}</color>", debugColor, _id);
            foreach (var progress in _progression)
            {
                debugColor = progress.Value.IsSolved ? "green" : "red";
                Debug.LogFormat("Solvable ID: <color={0}>{1}</color> | Is Solved: <color={0}>{2}</color>", debugColor, progress.Key, progress.Value.IsSolved);
            }

            if (IsCompleted = _progression.All((o) => o.Value.IsSolved))
            {
                GlobalEventBus.Raise(GlobalEvents.Game.ProgressionCompleted, _id);
                Debug.LogFormat("<color={0}>Progression of {1} has been completed</color>", debugColor, _id);
            }
        }

        private void InitializeProgression()
        {
            _progression.Clear();
            foreach (var solvableObject in _solvableDependenciesID)
                _progression.Add(solvableObject.ID, solvableObject);
        }
    }
}
