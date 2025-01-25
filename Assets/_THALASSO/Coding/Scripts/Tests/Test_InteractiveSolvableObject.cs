using ProgressionTracking;
using System;
using UnityEngine;

[Serializable]
public class Test_InteractiveSolvableObject : SolvableObjectBase, IAmInteractive
{
    [SerializeField]
    private SOUIntReference _id;
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

        Solve();
    }

    public override void Solve()
    {
        if (_meshRenderer.material.color == _baseColor)
        {
            _meshRenderer.material.color = Color.green;
            IsSolved = true;
            GlobalEventBus.Raise(GlobalEvents.Game.HasBeenSolved, _id.Value);
        }
        else
        {
            _meshRenderer.material.color = _baseColor;
            IsSolved = false;
        }
    }
}
