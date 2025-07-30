using UnityEngine;

public class MinionStateAttack : BaseState
{
    MinionStateMachine stateMachine;

    public MinionStateAttack(BaseStateMachine sm) : base(sm)
    {
        stateMachine = (MinionStateMachine) sm;
    }
    public override void thisStart()
    {
        base.thisStart();
    }
}
