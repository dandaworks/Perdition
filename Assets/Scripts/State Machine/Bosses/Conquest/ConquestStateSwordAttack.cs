using UnityEngine;

public class ConquestStateSwordAttack : BaseState
{
    ConquestStateMachine stateMachine;

    public ConquestStateSwordAttack(BaseStateMachine sm) : base(sm)
    {
        stateMachine = (ConquestStateMachine)sm;
    }
}
