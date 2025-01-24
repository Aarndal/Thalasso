using ProgressionTracking;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class Test_SOProgressionTracking : MonoBehaviour
{
    [SerializeField]
    private SO_ProgressionTracker _progressionTracker;

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

    private void Update()
    {
        if(_progressionTracker.IsCompleted)
            _meshRenderer.material.color = Color.green;
    }
}
