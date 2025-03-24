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

        protected void OnEnable()
        {
            foreach (var ambient in AmbientEventSender.Values)
            {
                ambient.OnEnable();
            }
        }

        private void InitAmbientEventSender()
        {
            foreach (var audioEvent in _wwiseEvents)
            {
                AkAmbient akAmbient = gameObject.AddComponent<AkAmbient>();

                akAmbient.data = audioEvent.Value;
                akAmbient.multiPositionTypeLabel = _positionType;
                akAmbient.useOtherObject = _playOnOtherObject;
                akAmbient.soundEmitterObject = _eventReceiver;
                akAmbient.triggerList.Clear();
                akAmbient.triggerList.Add(AkTriggerHandler.START_TRIGGER_ID);
                akAmbient.stopSoundOnDestroy = true;

                AmbientEventSender.TryAdd(audioEvent.ID, akAmbient);
            }
        }
#endif
    }
}
