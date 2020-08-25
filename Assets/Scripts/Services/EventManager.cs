using UnityEngine;
using System.Collections.Generic;

public enum EventType
{
    SimulationSceneLoaded,
    AgentGotCell,
    AgentGotTargetCell,
    SaveData
};    

public class EventManager 
{
    public static EventManager Instance => _instance;
    private static EventManager _instance = null;
    
    public EventManager()
    {
        if (_instance == null)
            _instance = this;
        else
            Debug.LogError("Попытка повторного создания синглтона - " + this.GetType().Name);
    }
    
    
    public delegate void OnEvent(EventType eventType, Component sender, object param = null);
    
    private Dictionary<EventType, List<OnEvent>> m_Listeners = new Dictionary<EventType, List<OnEvent>>();
    
    
    public void AddListener(EventType eventType, OnEvent listener)
    {
        if (m_Listeners.TryGetValue(eventType, out var listenList))
        {
            listenList.Add(listener);
            return;
        }

        listenList = new List<OnEvent> {listener};
        m_Listeners.Add(eventType, listenList);
    }
    
    public void RemoveListener(EventType eventType, OnEvent listener)
    {
        if (m_Listeners.TryGetValue(eventType, out var listenList))
            listenList.Remove(listener);
    }
    
    public void PostNotification(EventType eventType, Component sender, object param = null)
    {
        if (!m_Listeners.TryGetValue(eventType, out var listenList))
            return;
        foreach (var listener in listenList)
        {
            if (!listener.Equals(null))
                listener(eventType, sender, param);
        }
    }
}
