using System.Collections.Generic;
using UnityEngine;

namespace WwiseHelper
{
    public sealed class WwiseAmbienceEventHandler : WwiseEventHandler
    {
#if WWISE_2024_OR_LATER

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
#endif
    }
}
