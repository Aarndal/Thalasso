using System;
using UnityEngine;

[Serializable]
public abstract class TargetProvider : MonoBehaviour
{
    private Transform _target;

    public Transform Target
    {
        get => _target;
        protected set
        {
            if (_target != value && value == null)
            {
                TargetLost?.Invoke(_target);
                _target = value;
            }
            if (_target != value && value != null)
            {
                _target = value;
                NewTargetDetected?.Invoke(_target);
                Debug.Log($"New target detected: {_target.name}");
            }
        }
    }

    public bool HasTarget => Target != null && Target.gameObject.activeInHierarchy;

    public event Action<Transform> NewTargetDetected;
    public event Action<Transform> TargetLost;
    
    public abstract Transform GetTarget();
}
