using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
public class InteractableBall : MonoBehaviour, IAmInteractive, IAmMovable
{
    [SerializeField]
    private bool _isActivatable = true;
    [SerializeField]
    private float _kickForce = 10f;

    private Rigidbody _rigidbody = default;
    private Vector3 _kickDirection = Vector3.zero;
    private bool _isBeingKicked = false;

    public bool IsActivatable => _isActivatable;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        this.GetComponent<MeshRenderer>().material.color = Random.ColorHSV(0.0f, 1.0f, 0.75f, 1.0f, 0.5f, 1.0f);
    }

    private void FixedUpdate()
    {
        if (_isBeingKicked)
            Move();
    }

    public void Interact(Transform transform)
    {
        if (!IsActivatable)
            return;

        _kickDirection = transform.forward;
        _isBeingKicked = true;
    }

    public void Move()
    {
        _rigidbody.AddForce(_kickDirection * _kickForce, ForceMode.Impulse);
        _isBeingKicked = false;
    }
}
