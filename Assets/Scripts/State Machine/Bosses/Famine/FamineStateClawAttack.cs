using UnityEngine;

public class FamineStateClawAttack : BaseState
{
    FamineStateMachine stateMachine;
    public FamineStateClawAttack (BaseStateMachine sm) : base(sm)
    {
        stateMachine = (FamineStateMachine) sm;
    }

    public override void thisStart()
    {
        base.thisStart();
    }
}
