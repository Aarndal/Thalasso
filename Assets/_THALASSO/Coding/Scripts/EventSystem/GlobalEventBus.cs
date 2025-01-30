using System;
using System.Collections.Generic;

public static class GlobalEventBus
{
    public delegate void EventFunction(params object[] args);

    private static Dictionary<uint, List<EventFunction>> EventListener { get; set; }

    static GlobalEventBus() => EventListener = new();

    public static void Register(uint eventTypeID, EventFunction listenerEventFunction) // Subscribe
    {
        List<EventFunction> eventFunctions;

        if (!EventListener.TryGetValue(eventTypeID, out eventFunctions))
        {
            eventFunctions = new();
            EventListener.Add(eventTypeID, eventFunctions);
        }

        eventFunctions.Add(listenerEventFunction);
    }

    public static void Deregister(uint eventTypeID, EventFunction listenerEventFunction) // Unsubscribe
    {
        List<EventFunction> eventFunctions;

        if (!EventListener.TryGetValue(eventTypeID, out eventFunctions))
            return;

        eventFunctions.Remove(listenerEventFunction);
    }

    public static void Raise(uint eventTypeID, params object[] data) // Push/Publish
    {
        List<EventFunction> eventFunctions;

        if (!EventListener.TryGetValue(eventTypeID, out eventFunctions))
            return;

        List<EventFunction> eventFunctionsCopy = new(eventFunctions); //! Shallow copy to prevent editing the original list when iteration over it

        foreach (var eventFunction in eventFunctionsCopy)
            eventFunction(data);
    }
}
