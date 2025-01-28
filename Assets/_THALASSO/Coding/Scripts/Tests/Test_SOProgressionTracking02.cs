using ProgressionTracking;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class Test_SOProgressionTracking02 : MonoBehaviour
{
    [SerializeField]
    private SO_ProgressionTracker02 _progressionTracker;

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
        _meshRenderer.material.color = _progressionTracker.IsCompleted ? Color.green : _baseColor;
    }

    private void OnDisable()
    {
        GlobalEventBus.Deregister(GlobalEvents.Game.ProgressionCompleted, OnProgressionCompleted);
    }

    private void OnProgressionCompleted(object[] args)
    {
        if ((uint)args[0] == _progressionTracker.ID)
            _meshRenderer.material.color = Color.green;
    }
}
