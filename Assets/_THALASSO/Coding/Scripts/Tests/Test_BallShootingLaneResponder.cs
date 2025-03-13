using UnityEngine;

namespace TestSceneScripts
{
    internal class Test_BallShootingLaneResponder : ResponderBase
    {
        [SerializeField]
        private InteractiveDiscoBall _ball = default;
        [SerializeField]
        private float _impulse = 50f;

        public override void Respond(GameObject @gameObject, IAmTriggerable trigger)
        {
            _ball.Rigidbody.linearVelocity = Vector3.zero;
            _ball.transform.SetPositionAndRotation(transform.position, transform.rotation);
            _ball.SetKickForece(_impulse);
            _ball.Interact(transform);
        }
    }
}
