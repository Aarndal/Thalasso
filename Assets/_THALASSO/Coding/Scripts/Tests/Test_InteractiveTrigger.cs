using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class Test_InteractiveTrigger : InteractiveTrigger
{
    [SerializeField]
    private Color _baseColor = Color.white;

    private MeshRenderer _meshRenderer = default;

    protected override void Awake()
    {
        base.Awake();
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start() =>
        _meshRenderer.material.color = _baseColor;

    public override void Trigger(GameObject gameObject)
    {
        if (!IsTriggerable)
            return;

        _hasBeenTriggered?.Invoke(this);

        if (_meshRenderer.material.color == _baseColor)
            _meshRenderer.material.color = Color.red;
        else
            _meshRenderer.material.color = _baseColor;
    }
}
