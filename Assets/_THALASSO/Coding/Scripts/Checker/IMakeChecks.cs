using UnityEngine;

public interface IMakeChecks
{
    public bool IsActive { get; set; }

    bool Check(Transform transform);
}
