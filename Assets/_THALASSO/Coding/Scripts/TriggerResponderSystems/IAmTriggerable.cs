using System;

public interface IAmTriggerable
{
    event Action<IAmTriggerable> CannotBeTriggered;
    event Action<IAmTriggerable> HasBeenTriggered;

    void Trigger();
}
