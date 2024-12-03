using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SkillManagers : MonoBehaviour
{
    public static SkillManagers Instance;
    public List<UnityEvent> eventList = new List<UnityEvent>();

    public void AddEventToList(UnityEvent newEvent)
    {
        eventList.Add(newEvent);    
    }

    void Start()
    {
        Instance = this;
    }

    public void RaiseAllEvent()
    {
        foreach (var item in eventList)
        {
            item.Invoke();
        }
    }
    
}
