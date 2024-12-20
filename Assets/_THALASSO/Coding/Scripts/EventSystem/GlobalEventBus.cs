using System;
using System.Collections.Generic;

public static class GlobalEventBus
{
    public delegate void EventFunction(params object[] args);

    private static Dictionary<int, List<EventFunction>> EventListener { get; set; }

    static GlobalEventBus() => EventListener = new();

    public static void Register(int eventTypeID, EventFunction eventFunction) // Subscribe
    {
        List<EventFunction> eventFunctions;

        if (!EventListener.TryGetValue(eventTypeID, out eventFunctions))
        {
            eventFunctions = new();
            EventListener.Add(eventTypeID, eventFunctions);
        }

        eventFunctions.Add(eventFunction);
    }

    public static void Deregister(int eventTypeID, EventFunction eventFunction) // Unsubscribe
    {
        List<EventFunction> eventFunctions;

        if (!EventListener.TryGetValue(eventTypeID, out eventFunctions))
            return;

        eventFunctions.Remove(eventFunction);
    }

    public static void Raise(int eventTypeID, params object[] data) // Push
    {
        List<EventFunction> eventFunctions;

        if (!EventListener.TryGetValue(eventTypeID, out eventFunctions))
            return;

        List<EventFunction> eventFunctionsCopy = new(eventFunctions); //! Shallow copy to prevent editing the original list when iteration over it

        foreach (var eventFunction in eventFunctionsCopy)
            eventFunction(data);
    }
}
