using System;
using System.Collections.Generic;

public static class GlobalEventBus
{
    public delegate void EventFunction(params object[] eventArgs);

    private static Dictionary<uint, List<EventFunction>> EventListener { get; set; }

    static GlobalEventBus() => EventListener = new();

    public static void Register(uint eventTypeID, EventFunction listenerEventFunction) // Subscribe
    {
        if (!EventListener.TryGetValue(eventTypeID, out List<EventFunction> eventFunctions))
        {
            eventFunctions = new();
            EventListener.Add(eventTypeID, eventFunctions);
        }

        eventFunctions.Add(listenerEventFunction);
    }

    public static void Deregister(uint eventTypeID, EventFunction listenerEventFunction) // Unsubscribe
    {
        if (!EventListener.TryGetValue(eventTypeID, out List<EventFunction> eventFunctions))
            return;

        eventFunctions.Remove(listenerEventFunction);
    }

    public static void Raise(uint eventTypeID, params object[] eventArgs) // Push/Publish
    {
        if (!EventListener.TryGetValue(eventTypeID, out List<EventFunction> eventFunctions))
            return;

        List<EventFunction> eventFunctionsCopy = new(eventFunctions); //! Shallow copy to prevent editing the original list when iteration over it

        foreach (var eventFunction in eventFunctionsCopy)
            eventFunction(eventArgs);
    }
}
