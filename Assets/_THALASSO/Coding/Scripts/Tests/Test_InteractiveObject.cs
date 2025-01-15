using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class Test_InteractiveObject : MonoBehaviour, IAmInteractive
{
    [SerializeField]
    private Color _baseColor = Color.white;

    private MeshRenderer _meshRenderer = default;

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
        if (_meshRenderer.material.color == _baseColor)
            _meshRenderer.material.color = Color.red;
        else
            _meshRenderer.material.color = _baseColor;
    }
}