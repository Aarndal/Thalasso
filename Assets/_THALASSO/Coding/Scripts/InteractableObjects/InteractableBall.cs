using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
public class InteractableBall : MonoBehaviour, IAmInteractive, IAmMovable
{
    [SerializeField]
    private float _kickForce = 10f;

    private Rigidbody _rigidbody = default;
    private Vector3 _kickDirection = Vector3.zero;
    private bool _isBeingKicked = false;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (_isBeingKicked)
            Move();
    }

    public void Interact(Transform transform)
    {
        _kickDirection = transform.forward;
        _isBeingKicked = true;
    }

    public void Move()
    {
        _rigidbody.AddForce(_kickDirection * _kickForce, ForceMode.Impulse);
        _isBeingKicked = false;
    }
}
