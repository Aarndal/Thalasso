using ProgressionTracking;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class Test_ProgressionTracking : MonoBehaviour
{
    [SerializeField]
    private Color _baseColor = Color.white;

    private ProgressionTracker _progressionTracker;
    private MeshRenderer _meshRenderer = default;
    
    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _progressionTracker = GetComponent<ProgressionTracker>();
    }

    private void Start()
    {
        _meshRenderer.material.color = _baseColor;
    }

    private void Update()
    {
        if(_progressionTracker.IsCompleted)
            _meshRenderer.material.color = Color.green;
    }
}
