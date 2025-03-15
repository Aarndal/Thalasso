using System.Collections.Generic;
using UnityEngine;

namespace WwiseHelper
{
    public class WwiseSwitchResponder : Responder
    {
#if WWISE_2024_OR_LATER
        [Space(5)]

        [Header("Wwise Settings")]
        [SerializeField]
        private AK.Wwise.Switch[] _wwiseSwitch = default;

        public HashSet<AK.Wwise.Switch> AudioSwitches = new();
#endif

        protected override void Awake()
        {
            base.Awake();

#if WWISE_2024_OR_LATER
            foreach (var audioSwitch in _wwiseSwitch)
            {
                AudioSwitches?.Add(audioSwitch);
            }
#endif
        }

        public override void Respond(GameObject triggeringObject, ResponderState responderState)
        {
#if WWISE_2024_OR_LATER
            if (triggeringObject.TryGetComponent<AkGameObj>(out _))
            {
                foreach (var audioSwitch in AudioSwitches)
                {
                    audioSwitch.SetValue(triggeringObject);
                }
            }
#endif
        }
    }
}
