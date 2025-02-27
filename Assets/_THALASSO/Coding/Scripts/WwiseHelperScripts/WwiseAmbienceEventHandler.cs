using System.Collections.Generic;
using UnityEngine;

namespace WwiseHelper
{
    public sealed class WwiseAmbienceEventHandler : WwiseEventHandler
    {
        protected override void Awake()
        {
            areEnvironmentAware = true;
            base.Awake();
        }

        private void OnEnable()
        {

        }

        private void OnDisable()
        {
           
        }
    }
}
