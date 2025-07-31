using UnityEngine;

public class ConquestStateIdle : BossStateIdle
{
    ConquestStateMachine stateMachine;

    public ConquestStateIdle(BaseStateMachine sm) : base(sm)
    {
        stateMachine = (ConquestStateMachine)sm;
    }

    public override void thisUpdate()
    {
        base.thisUpdate();
        stateMachine.transform.position = stateMachine.IdlePosition;
    }

    public override void thisEnd()
    {
        base.thisEnd();

        stateMachine.transform.position = stateMachine.StartPosition;
    }
}
