using System.Collections.Generic;

public static class EventBus<T> where T : IEvent
{
    static readonly HashSet<IEventBinding<T>> Bindings = new();

    public static void Register(EventBinding<T> binding) => Bindings.Add(binding);
    public static void Deregister(EventBinding<T> binding) => Bindings.Remove(binding);

    public static void Raise(T @event)
    {
        foreach(var binding in Bindings)
        {
            binding.OnEvent.Invoke(@event);
            binding.OnEventNoArgs.Invoke();
        }
    }
}
