using ProgressionTracking;
using System;
using UnityEngine;

[Serializable]
public class Test_InteractiveSolvableObject02 : SolvableObjectBase, IAmInteractive
{
    [SerializeField]
    private SO_SolvableObject _solvableObject;
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
        _meshRenderer.material.color = _solvableObject.IsSolved ? Color.green : _baseColor;
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
            _solvableObject.IsSolved = true;
        }
    }
}
