using System;
using System.Collections;
using UnityEngine;

[Serializable]
[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
public class InteractiveDiscoBall : MonoBehaviour, IAmInteractive, IAmMovable
{
    [SerializeField]
    private bool _isActivatable = true;
    [SerializeField, Range(1f, 100f)]
    private float _kickForce = 10f;
    [SerializeField]
    private float _defaultDiscoTime = 5.0f;

    private Rigidbody _rigidbody = default;
    private Vector3 _kickDirection = Vector3.zero;
    private float _discoTime = 0.0f;

    public bool IsActivatable => _isActivatable;
    public float KickForce
    {
        get => _kickForce;
        private set
        {
            if (value != _kickForce)
            {
                if (value >= 100f)
                    _kickForce = 100f;
                else if (value <= 1f)
                    _kickForce = 1f;
                else
                    _kickForce = value;
            }
        }
    }
    public Rigidbody Rigidbody => _rigidbody;

    private event Action IsKicked;

    private void Awake()
    {
        if (LayerMask.LayerToName(gameObject.layer) != "InteractiveObject" && LayerMask.NameToLayer("InteractiveObject") == 20)
            gameObject.layer = LayerMask.NameToLayer("InteractiveObject");

        _rigidbody = GetComponent<Rigidbody>();

        gameObject.GetComponent<MeshRenderer>().material.color = UnityEngine.Random.ColorHSV(0.0f, 1.0f, 0.75f, 1.0f, 0.5f, 1.0f);
    }

    private void OnEnable() => IsKicked += Move;

    private void Start() => _discoTime = _defaultDiscoTime;

    private void OnDisable() => IsKicked -= Move;

    public void Interact(Transform transform)
    {
        if (!IsActivatable)
            return;

        _kickDirection = transform.forward;
        IsKicked?.Invoke();
    }

    public void Move()
    {
        _rigidbody.AddForce(_kickDirection * _kickForce, ForceMode.Impulse);

        if (_discoTime < _defaultDiscoTime)
            return;

        StartCoroutine(Disco());
    }

    public void SetKickForece(float kickForce) => KickForce = kickForce;

    private IEnumerator Disco()
    {
        while (_discoTime >= 0.0f)
        {
            GetComponent<MeshRenderer>().material.color = UnityEngine.Random.ColorHSV(0.0f, 1.0f, 0.75f, 1.0f, 0.5f, 1.0f);

            _discoTime -= 0.1f;

            yield return new WaitForSeconds(0.1f);
        }

        _discoTime = _defaultDiscoTime;
        yield return null;
    }
}
