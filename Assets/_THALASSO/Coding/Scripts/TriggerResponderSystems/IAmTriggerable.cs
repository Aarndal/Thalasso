using System;

public interface IAmTriggerable
{
    event Action<IAmTriggerable> HasBeenTriggered;

    bool Trigger();
}
