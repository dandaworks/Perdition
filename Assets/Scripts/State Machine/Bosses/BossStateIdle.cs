using UnityEngine;

public class BossStateIdle : BaseState
{
    BossStateMachine stateMachine;

    public BossStateIdle (BaseStateMachine sm) : base(sm)
    {
        stateMachine = (BossStateMachine) sm;
    }

    public override void thisStart()
    {
        base.thisStart();
        if (stateMachine.animator)
        {
            stateMachine.animator.Play("Idle");
        }
    }

    public override void thisUpdate()
    {
        base.thisUpdate();

        if (stateMachine.player == null) return;

        float distance = Vector3.Distance(stateMachine.transform.position, stateMachine.player.position);

        if (distance <= stateMachine.detectionRange)
        {
            stateMachine.ChangeState(stateMachine.stateFollow);
        }
    }
}
