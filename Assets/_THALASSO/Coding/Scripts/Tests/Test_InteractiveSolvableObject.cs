using ProgressionTracking;
using UnityEngine;

public class Test_InteractiveSolvableObject : SolvableObject, IAmOperable
{
    [SerializeField]
    private bool _isActivatable = true;
    [SerializeField]
    private Color _baseColor = Color.red;

    private MeshRenderer _meshRenderer = default;

    public bool IsOperable { get => _isActivatable; private set => _isActivatable = value; }

    private void Awake()
    {
        if (gameObject.layer != (int)Layers.InteractiveObject)
            gameObject.layer = (int)Layers.InteractiveObject;

        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Reset()
    {
        gameObject.layer = (int)Layers.InteractiveObject;
    }

    private void Start()
    {
        _meshRenderer.material.color = IsSolved ? Color.green : _baseColor;

        IsOperable = !IsSolved;
    }

    public void Operate(Transform transform)
    {
        if (IsSolved)
            IsOperable = false;

        if (!IsOperable)
            return;

        Solve();
    }

    public override bool Solve() => IsSolved = IsPlatformActivated();

    private bool IsPlatformActivated()
    {
        if (_meshRenderer.material.color == _baseColor)
        {
            _meshRenderer.material.color = Color.green;
            return true;
        }

        return false;
    }
}
