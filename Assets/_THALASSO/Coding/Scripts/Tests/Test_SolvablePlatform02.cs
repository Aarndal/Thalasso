using System;
using UnityEngine;

namespace ProgressionTracking
{
    [Serializable]
    public class Test_SolvablePlatform02 : SolvableObjectBase
    {
        [SerializeField]
        private SO_SolvableObject _solvableObject;
        [SerializeField]
        private SO_ProgressionTracker02 _progressionTracker;

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
            _meshRenderer.material.color = _solvableObject.IsSolved ? Color.green : Color.red;
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

        public override void Solve()
        {
            _meshRenderer.material.color = Color.green;
            _solvableObject.IsSolved = true;
            //GlobalEventBus.Raise(GlobalEvents.Game.HasBeenSolved, _solvableObject.ID);
        }

        private void OnProgressionCompleted(object[] args)
        {
            if ((uint)args[0] == _progressionTracker.ID)
                _meshRenderer.material.color = Color.white;
        }
    }
}
