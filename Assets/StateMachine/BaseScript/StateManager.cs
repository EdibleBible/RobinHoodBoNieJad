using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateManager<EState> : MonoBehaviour where EState : Enum
{
    public Dictionary<EState,BaseState<EState>> state = new Dictionary<EState, BaseState<EState>>();
    public BaseState<EState> currentState;

    public bool isTransitioningState = false;

    public virtual void Start() 
    {
        currentState.EnterState();
    }
    public virtual void Update()
    {
        EState nextStateKey = currentState.GetNextState();
        if(!isTransitioningState && nextStateKey.Equals(currentState.stateKey))
        {
            currentState.UpdateState();
        }
        else if(!isTransitioningState)
        {
            TransitionToState(nextStateKey);
        }
    }

    public void TransitionToState(EState stateKey)
    {
        isTransitioningState = true;
        currentState.ExitState();
        currentState = state[stateKey];
        currentState.EnterState();
        isTransitioningState = false;
    }

    public virtual void OnTriggerEnter(Collider other)
    { 
        currentState.OnTriggerEnterState(other);
    }
    public virtual void OnTriggerStay(Collider other)
    { 
        currentState.OnTriggerStayState(other);
    }
    public virtual void OnTriggerExit(Collider other)
    { 
        currentState.OnTriggerExitState(other);
    }
}
