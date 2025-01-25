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

    private void OnEnable()
    {
        GlobalEventBus.Register(GlobalEvents.Game.ProgressionCompleted, OnProgressionCompleted);
    }

    private void Start()
    {
        if (_progressionTracker.IsCompleted)
            _meshRenderer.material.color = Color.green;
        else
            _meshRenderer.material.color = _baseColor;
    }

    private void OnDisable()
    {
        GlobalEventBus.Register(GlobalEvents.Game.ProgressionCompleted, OnProgressionCompleted);
    }

    private void OnProgressionCompleted(object[] args)
    {
        if ((uint)args[0] == _progressionTracker.ID)
            _meshRenderer.material.color = Color.green;
    }
}
