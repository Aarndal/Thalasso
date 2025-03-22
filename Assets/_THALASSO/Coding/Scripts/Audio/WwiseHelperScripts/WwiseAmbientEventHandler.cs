using System.Collections.Generic;
using UnityEngine;

namespace WwiseHelper
{
    public class WwiseAmbientEventHandler : WwiseEventHandler
    {
#if WWISE_2024_OR_LATER
        [SerializeField]
        private MultiPositionTypeLabel _positionType = MultiPositionTypeLabel.Simple_Mode;

        public readonly Dictionary<uint, AkAmbient> AmbientEventSender = new();

        protected override void Awake()
        {
            _playOnOtherObject = false;

            base.Awake();

            InitAmbientEventSender();
        }

        private void InitAmbientEventSender()
        {
            foreach (var audioEvent in _wwiseEvents)
            {
                AkAmbient akAmbient = gameObject.AddComponent<AkAmbient>();

                akAmbient.data = audioEvent.Value;
                akAmbient.useOtherObject = _playOnOtherObject;

                if (_positionType == MultiPositionTypeLabel.MultiPosition_Mode)
                {
                    if (!AkAmbient.multiPosEventTree.ContainsKey(audioEvent.ID))
                    {
                        AkMultiPosEvent akMultiPosEvent = new ();
                        akMultiPosEvent.list.Add(akAmbient);
                        AkAmbient.multiPosEventTree.Add(audioEvent.ID, akMultiPosEvent);
                    }
                }
                
                akAmbient.multiPositionTypeLabel = _positionType;
                akAmbient.triggerList.Clear();
                akAmbient.triggerList.Add(AkTriggerHandler.START_TRIGGER_ID);
                akAmbient.stopSoundOnDestroy = true;

                AmbientEventSender.TryAdd(audioEvent.ID, akAmbient);
            }
        }
#endif
    }
}
