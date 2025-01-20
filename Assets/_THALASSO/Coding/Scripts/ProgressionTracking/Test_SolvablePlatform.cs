using ProgressionTracking;
using System;
using UnityEngine;

namespace ProgressionTracking
{
    [Serializable]
    public class Test_SolvablePlatform : SolvableObjectBase
    {
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
            _meshRenderer.material.color = Color.red;
        }

        private void OnTriggerEnter(Collider other)
        {
            Solve();
        }

        private void OnDisable()
        {
            GlobalEventBus.Deregister(GlobalEvents.Game.ProgressionCompleted, OnProgressionCompleted);
        }

        public override void Solve()
        {
            _meshRenderer.material.color = Color.green;
            IsSolved = true;
        }

        private void OnProgressionCompleted(object[] args)
        {
            if ((uint)args[0] == 101)
                _meshRenderer.material.color = Color.white;
        }
    }
}
