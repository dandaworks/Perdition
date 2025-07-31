using UnityEngine;

public class PlayerStateWalk : BaseState
{
    PlayerStateMachine stateMachine;

    public PlayerStateWalk(PlayerStateMachine sm) : base(sm)
    {
        stateMachine = sm;
    }

    public override void thisStart()
    {
        base.thisStart();

        stateMachine.animator.Play("Walk");
    }

    public override void thisUpdate()
    {
        base.thisUpdate();

        if (stateMachine.GetMoveInput() == Vector2.zero) { stateMachine.ChangeState(stateMachine.stateIdle); }

        MovePlayer();
    }

    public void MovePlayer()
    {
        stateMachine.GetController().Move(stateMachine.moveSpeed * Time.deltaTime * stateMachine.GetMoveDirection());

        if (stateMachine.GetMoveDirection() != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(stateMachine.GetMoveDirection(), Vector3.up);
            stateMachine.transform.rotation = Quaternion.Slerp(stateMachine.transform.rotation, toRotation, 10f * Time.deltaTime);
        }
    }
}
