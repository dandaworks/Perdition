using UnityEngine;

public class PlayerStateDead : BaseState
{
    PlayerStateMachine stateMachine;

    public PlayerStateDead(PlayerStateMachine sm) : base(sm)
    {
        stateMachine = sm;
    }

    public override void thisStart()
    {
        base.thisStart();

        stateMachine.animator.Play("Dead");

        stateMachine.SetIsAlive(false);
        stateMachine.deathUI.SetActive(true);
    }
}
