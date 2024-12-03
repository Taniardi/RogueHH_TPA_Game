using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[CreateAssetMenu(fileName = "IntEventScriptableObject", menuName = "ScriptableObjects/IntEventChannelManager")]
public class IntEventChannel : ScriptableObject
{
    public event UnityAction<int> OnEventRaised;

    public void RaiseEvent(int value)
    {   
        if(OnEventRaised != null)
        {
            OnEventRaised.Invoke(value);    
        }
    }

}
