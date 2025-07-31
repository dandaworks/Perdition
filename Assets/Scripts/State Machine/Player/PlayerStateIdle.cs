using UnityEngine;

public class PlayerStateIdle : BaseState
{
    PlayerStateMachine stateMachine;

    public PlayerStateIdle(PlayerStateMachine sm) : base(sm)
    {
        stateMachine = sm;
    }

    public override void thisStart()
    {
        base.thisStart();

        Debug.Log("Idle");

        stateMachine.animator.Play("Idle");
    }

    public override void thisUpdate()
    {
        base.thisUpdate();

        if (stateMachine.GetMoveInput() != Vector2.zero) { stateMachine.ChangeState(stateMachine.stateWalk); }
    }
}
