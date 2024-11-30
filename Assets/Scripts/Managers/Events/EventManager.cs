using System;
using System.Collections.Generic;

public class EventManager
{
    private static EventManager instance;
    public static EventManager Instance => instance ??= new EventManager();
    private readonly Dictionary<string, Action<object>> eventDictionary = new Dictionary<string, Action<object>>();

    private string GetEventKey<T>(T eventName) where T : Enum
    {
        return $"{typeof(T).Name}_{eventName}";
    }

    public void Subscribe<T>(T eventName, Action<object> listener) where T : Enum
    {
        string key = GetEventKey(eventName);

        if (!eventDictionary.ContainsKey(key))
            eventDictionary[key] = delegate { };

        eventDictionary[key] += listener;
    }

    public void Unsubscribe<T>(T eventName, Action<object> listener) where T : Enum
    {
        string key = GetEventKey(eventName);

        if (eventDictionary.ContainsKey(key))
            eventDictionary[key] -= listener;
    }

    public void TriggerEvent<T>(T eventName, object eventData = null) where T : Enum
    {
        string key = GetEventKey(eventName);

        if (eventDictionary.ContainsKey(key))
            eventDictionary[key]?.Invoke(eventData);
    }
}