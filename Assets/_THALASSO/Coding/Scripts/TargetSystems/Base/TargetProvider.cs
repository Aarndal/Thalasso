using System;
using UnityEngine;

[Serializable]
public abstract class TargetProvider : MonoBehaviour
{
    private Transform _target = default;

    public Transform Target
    {
        get => _target;
        protected set
        {
            if (value != _target)
            {
                TargetChanged?.Invoke(_target, value);

                //if (value == null)
                //    Debug.LogFormat($"Lost target: {_target.gameObject.name}");

                //if (value != null)
                //    Debug.LogFormat($"New target detected: {value.gameObject.name}");

                _target = value;
            }
        }
    }

    public bool HasTarget => Target != null && Target.gameObject.activeInHierarchy;

    public event Action<Transform, Transform> TargetChanged;

    public abstract Transform GetTarget();
}
