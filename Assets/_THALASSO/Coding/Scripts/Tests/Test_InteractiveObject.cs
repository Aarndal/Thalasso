using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class Test_InteractiveObject : MonoBehaviour, IAmInteractive
{
    [SerializeField]
    private bool _isActivatable = true;
    [SerializeField]
    private Color _baseColor = Color.white;

    private MeshRenderer _meshRenderer = default;

    public bool IsActivatable => _isActivatable;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        _meshRenderer.material.color = _baseColor;
    }

    public void Interact(Transform transform)
    {
        if (!IsActivatable)
            return;

        if (_meshRenderer.material.color == _baseColor)
            _meshRenderer.material.color = Color.red;
        else
            _meshRenderer.material.color = _baseColor;
    }
}