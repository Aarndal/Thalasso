using System.Collections.Generic;
using UnityEngine;

namespace WwiseHelper
{
    public class WwiseAmbientEventResponder : WwiseEventResponder
    {
#if WWISE_2024_OR_LATER
        [SerializeField]
        private AkMultiPositionType _akMultiPositionType = AkMultiPositionType.MultiPositionType_SingleSource;

        public readonly Dictionary<AK.Wwise.Event, AkAmbient> AmbientComponents = new();

        protected override void Awake()
        {
            base.Awake();

            AkAmbient[] akAmbients = GetComponents<AkAmbient>();

            foreach (var wwiseEvent in _wwiseEvents)
            {
                if (akAmbients.Length > 0)
                {
                    foreach (var akAmbient in akAmbients)
                    {
                        if (akAmbient.data == wwiseEvent)
                        {
                            if (AmbientComponents.TryAdd(wwiseEvent, akAmbient))
                                break;
                            else
                                Debug.LogWarningFormat("{0} already contains a {2} component that plays {1} event!", gameObject.name, wwiseEvent.Name, akAmbient.name);
                        }
                    }
                }

                if (AmbientComponents.ContainsKey(wwiseEvent))
                    continue;

                AkAmbient temp = gameObject.AddComponent<AkAmbient>();
                temp.data = wwiseEvent;

                if (!AmbientComponents.TryAdd(wwiseEvent, temp))
                    Debug.LogWarningFormat("{0} already contains a {2} component that plays {1} event!", gameObject.name, wwiseEvent.Name, temp.name);
            }

            if (AmbientComponents.Count > 0)
            {
                foreach (var ambientComponent in AmbientComponents.Values)
                {
                    ambientComponent.triggerList.Clear();
                    ambientComponent.triggerList.Add(AkTriggerHandler.START_TRIGGER_ID);

                    ambientComponent.MultiPositionType = _akMultiPositionType;
                }
            }
        }
#endif
    }
}
