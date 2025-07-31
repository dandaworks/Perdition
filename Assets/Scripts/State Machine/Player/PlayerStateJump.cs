using UnityEngine;

public class PlayerStateJump : PlayerStateWalk
{
    PlayerStateMachine stateMachine;

    public PlayerStateJump(PlayerStateMachine sm) : base(sm)
    {
        stateMachine = sm;
    }

    float lastY;

    public override void thisStart()
    {
        Vector3 currentVelocity = new Vector3(stateMachine.GetVelocity().x, Mathf.Sqrt(stateMachine.jumpHeight * -2f * stateMachine.gravity), stateMachine.GetVelocity().z);

        lastY = float.MinValue;

        stateMachine.SetVelocity(currentVelocity);
        stateMachine.animator.Play("Jump");
    }
    public override void thisUpdate()
    {
        MovePlayer();

        if (lastY > stateMachine.transform.position.y && Physics.Raycast(stateMachine.transform.position, Vector3.down, 2f, LayerMask.GetMask("Environment")))
        {
            stateMachine.ChangeState(stateMachine.InitialState());
        }

        lastY = stateMachine.transform.position.y;
    }
}
