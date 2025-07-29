using UnityEngine;

public class BaseState
{
    BaseStateMachine stateMachine;

    public BaseState(BaseStateMachine sm)
    {
        stateMachine = sm;
    }


    public virtual void thisStart()
    {
    }

    public virtual void thisUpdate()
    {

    }

    public virtual void thisFixedUpdate()
    {

    }

    public virtual void thisLateUpdate()
    {

    }

    public virtual void thisEnd()
    {

    }
}
