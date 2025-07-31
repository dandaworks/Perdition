using UnityEngine;

public class FamineStateClawAttack : BaseState
{
    FamineStateMachine stateMachine;
    public FamineStateClawAttack (BaseStateMachine sm) : base(sm)
    {
        stateMachine = (FamineStateMachine) sm;
    }

    float timer;
    float maxTimer = 2f;

    public override void thisStart()
    {
        base.thisStart();

        timer = maxTimer;

        stateMachine.animator.Play("ClawAttack");
        stateMachine.handAttack.SetActive(true);
    }

    public override void thisUpdate()
    {
        base.thisUpdate();

        if (maxTimer - timer > .42f)
        {
            stateMachine.handAttack.SetActive(false);
        }

        if (timer <= 0)
        {
            stateMachine.ChangeState(stateMachine.stateFollow);
        }
        else
        {
            timer -= Time.deltaTime;
        }
    }

    public override void thisEnd()
    {
        base.thisEnd();
        stateMachine.handAttack.SetActive(false);
    }
}
