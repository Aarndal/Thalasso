using UnityEngine;

[DisallowMultipleComponent]
public class Test_ColliderTrigger : ColliderTriggerBase
{
    public override void Trigger() => _hasBeenTriggered?.Invoke(this);
}
