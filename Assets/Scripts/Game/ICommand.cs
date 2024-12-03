using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICommand 
{
    public bool IsEndTurn { get; set; }
    public void Task()
    {
        
    }
}
