using UnityEngine;

public class Test_Responder : ResponderBase
{
    [SerializeField]
    private Color _baseColor = Color.black;

    private MeshRenderer _meshRenderer = default;

    protected override void Awake()
    {
        base.Awake();
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    protected virtual void Start() =>
        _meshRenderer.material.color = _baseColor;

    public override bool Respond(IAmTriggerable trigger)
    {
        if (_meshRenderer.material.color == _baseColor)
            _meshRenderer.material.color = Color.white;
        else
            _meshRenderer.material.color = _baseColor;

        return true;
    }
}
