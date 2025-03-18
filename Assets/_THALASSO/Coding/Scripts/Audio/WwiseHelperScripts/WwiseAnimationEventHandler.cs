using UnityEngine;

/* Plays a WwiseEvent when the referenced AnimationEventBroadcaster sends an AnimationEvent.
 * The EventArgs of the AnimationEvent must contain a string parameter equal to the name of one of the WwiseEvents registered in the WwiseEventHandler's AudioEvents dictionary.
 */
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
        }

        private void OnDisable()
        {
            if (AnimationEventBroadcaster != null)
                AnimationEventBroadcaster.AnimationEventTriggered -= OnAnimationEventTriggered;
        }

        private void OnAnimationEventTriggered(AnimationEvent eventArgs)
        {
#if WWISE_2024_OR_LATER
            if (AudioEvents.ContainsKey(eventArgs.stringParameter))
            {
                AudioEvents[eventArgs.stringParameter].Play(_akGameObject);
            }
#endif
        }
    }
}
