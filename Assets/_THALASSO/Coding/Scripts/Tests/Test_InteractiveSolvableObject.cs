using ProgressionTracking;
using System;
using UnityEngine;

[Serializable]
public class Test_InteractiveSolvableObject : SolvableObjectBase, IAmInteractive
{
    [SerializeField]
    private bool _isActivatable = true;
    [SerializeField]
    private Color _baseColor = Color.red;

    private MeshRenderer _meshRenderer = default;

    public bool IsActivatable { get => _isActivatable; private set => _isActivatable = value; }

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        _meshRenderer.material.color = IsSolved ? Color.green : _baseColor;

        IsActivatable = !IsSolved;
    }

    public void Interact(Transform transform)
    {
        if (!IsActivatable)
            return;

        Solve();
    }

    public override bool Solve() => IsSolved = IsPlatformActivated();

    private bool IsPlatformActivated()
    {
        if (_meshRenderer.material.color == _baseColor)
        {
            _meshRenderer.material.color = Color.green;
            IsActivatable = false;
            return true;
        }

        return false;
    }
}
