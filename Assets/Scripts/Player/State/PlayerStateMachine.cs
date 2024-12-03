using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine 
{
    public PlayerState currentState { get; set; }

    public void Initiate(PlayerState state)
    {
        currentState = state;
        currentState.EnterState();
    }

    public void ChangeState(PlayerState newState)
    {
        currentState.ExitState();
        currentState = newState;
        currentState.EnterState();  
    }

}
