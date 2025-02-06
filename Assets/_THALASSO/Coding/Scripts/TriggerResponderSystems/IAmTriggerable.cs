using System;

public interface IAmTriggerable
{
    event Action<IAmTriggerable> HasBeenTriggered;

    void Trigger();
}
