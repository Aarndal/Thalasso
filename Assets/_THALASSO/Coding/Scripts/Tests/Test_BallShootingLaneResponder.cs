using UnityEngine;

namespace TestSceneScripts
{
    internal class Test_BallShootingLaneResponder : Responder
    {
        [SerializeField]
        private OperableDiscoBall _ball = default;
        [SerializeField]
        private float _impulse = 50f;

        public override void Respond(GameObject @gameObject, ResponderState triggerState)
        {
            _ball.Rigidbody.linearVelocity = Vector3.zero;
            _ball.transform.SetPositionAndRotation(transform.position, transform.rotation);
            _ball.SetKickForece(_impulse);
            _ball.Operate(transform);
        }
    }
}
