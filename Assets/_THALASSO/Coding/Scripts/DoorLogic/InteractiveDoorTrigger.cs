using UnityEngine;

public class InteractiveDoorTrigger : InteractiveTrigger
{
    [SerializeField, TextArea]
    protected string _messageText = "";

    public override void Trigger()
    {
        if (!IsTriggerable)
        {
            _cannotBeTriggered?.Invoke(gameObject, _messageText);
            return;
        }

        _hasBeenTriggered?.Invoke(this);
    }
}
