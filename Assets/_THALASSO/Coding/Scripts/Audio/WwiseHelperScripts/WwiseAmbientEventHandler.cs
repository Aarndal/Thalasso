using System.Collections.Generic;
using UnityEngine;

namespace WwiseHelper
{
    public class WwiseAmbientEventHandler : WwiseEventHandler
    {
#if WWISE_2024_OR_LATER
        [SerializeField]
        private MultiPositionTypeLabel _positionType = MultiPositionTypeLabel.Simple_Mode;

        public readonly Dictionary<uint, AkAmbient> AmbientEventSender = default;

        protected override void Awake()
        {
            _playOnOtherObject = false;

            base.Awake();

            InitAmbientEventSender();
        }

        //protected void OnEnable()
        //{
        //    foreach (var broadcaster in AmbientEventSender.Values)
        //    {
        //        broadcaster.data.Post(_eventReceiver);
        //    }
        //}

        //protected void OnDisable()
        //{
        //    foreach (var broadcaster in AmbientEventSender.Values)
        //    {
        //        broadcaster.data.Stop(_eventReceiver);
        //    }
        //}

        private void InitAmbientEventSender()
        {
            foreach (var audioEvent in _wwiseEvents)
            {
                AkAmbient akAmbient = gameObject.AddComponent<AkAmbient>();

                akAmbient.data = audioEvent.Value;

                if (_positionType == MultiPositionTypeLabel.MultiPosition_Mode)
                {
                    AkMultiPosEvent akMultiPosEvent = new ();
                    akMultiPosEvent.list.Add(akAmbient);
                    AkAmbient.multiPosEventTree.TryAdd(akAmbient.data.Id, akMultiPosEvent);
                }
                
                akAmbient.multiPositionTypeLabel = _positionType;

                akAmbient.stopSoundOnDestroy = true;
                akAmbient.HandleEvent(_eventReceiver);
                akAmbient.triggerList.Clear();
                akAmbient.triggerList.Add(AkTriggerHandler.START_TRIGGER_ID);

                AmbientEventSender.TryAdd(audioEvent.ID, akAmbient);
            }
        }
#endif
    }
}
