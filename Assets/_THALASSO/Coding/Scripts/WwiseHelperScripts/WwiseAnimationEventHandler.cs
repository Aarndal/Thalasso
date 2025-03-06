using UnityEngine;

namespace WwiseHelper
{
    public sealed class WwiseAnimationEventHandler : WwiseEventHandler
    {
        [SerializeField]
        private AnimationEventBroadcaster _animationEventBroadcaster = default;

        public AnimationEventBroadcaster AnimationEventBroadcaster => _animationEventBroadcaster;

        protected override void Awake()
        {
            base.Awake();

            if (_animationEventBroadcaster == null)
            {
                Debug.LogErrorFormat("<color=cyan>{0}</color> missing <color=red>{1} reference</color>!", gameObject.name, _animationEventBroadcaster);
                return;
            }
        }

        private void OnEnable()
        {
            if (AnimationEventBroadcaster != null)
                AnimationEventBroadcaster.AnimationEventTriggered += OnAnimationEventTriggered;

#if WWISE_2024_OR_LATER
            GlobalEventBus.Register(GlobalEvents.Player.GroundSoundMaterialChanged, OnGroundSoundMaterialChanged);
#endif
        }

        private void OnDisable()
        {
            if (AnimationEventBroadcaster != null)
                AnimationEventBroadcaster.AnimationEventTriggered -= OnAnimationEventTriggered;

#if WWISE_2024_OR_LATER
            GlobalEventBus.Deregister(GlobalEvents.Player.GroundSoundMaterialChanged, OnGroundSoundMaterialChanged);
#endif
        }

        private void OnAnimationEventTriggered(AnimationEvent eventArgs)
        {
#if WWISE_2024_OR_LATER
            if (AudioEvents.ContainsKey(eventArgs.stringParameter))
            {
                AudioEvents[eventArgs.stringParameter].Post(_akGameObject.gameObject);
            }
#endif
        }

#if WWISE_2024_OR_LATER
        private void OnGroundSoundMaterialChanged(object[] args)
        {
            if (args[0] is AK.Wwise.Switch currentSoundMaterial)
            {
                currentSoundMaterial.SetValue(_akGameObject.gameObject);
            }
        }
#endif
    }
}
