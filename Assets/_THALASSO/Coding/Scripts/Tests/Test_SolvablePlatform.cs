using System;
using UnityEngine;

namespace ProgressionTracking
{
    [Serializable]
    public class Test_SolvablePlatform : SolvableObject
    {
        [SerializeField]
        private SO_ProgressionTracker _progressionTracker = default;

        private Collider _collider = default;
        private MeshRenderer _meshRenderer = default;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _meshRenderer = GetComponent<MeshRenderer>();
        }

        private void OnEnable()
        {
            GlobalEventBus.Register(GlobalEvents.Game.ProgressionCompleted, OnProgressionCompleted);
        }


        private void Start()
        {
            _collider.isTrigger = true;
            _meshRenderer.material.color = IsSolved ? Color.green : Color.red;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.CompareTag("Player"))
                Solve();
        }

        private void OnDisable()
        {
            GlobalEventBus.Deregister(GlobalEvents.Game.ProgressionCompleted, OnProgressionCompleted);
        }

        public override bool Solve() => IsSolved = IsPlatformActivated();

        private bool IsPlatformActivated()
        {
            _meshRenderer.material.color = Color.green;
            return true;
        }

        private void OnProgressionCompleted(object[] args)
        {
            if ((uint)args[0] == _progressionTracker.ID)
                _meshRenderer.material.color = Color.cyan;
        }
    }
}
