using UnityEngine;

public class MinionStateAttack : BaseState
{
    MinionStateMachine stateMachine;

    public MinionStateAttack(BaseStateMachine sm) : base(sm)
    {
        stateMachine = (MinionStateMachine) sm;
    }

    float timer;
    float maxTimer = 1f;

    public override void thisStart()
    {
        base.thisStart();

        timer = maxTimer;

        stateMachine.animator.Play("Melee");
        stateMachine.attack.SetActive(true);

        Debug.Log("Start Attack");
    }

    public override void thisUpdate()
    {
        base.thisUpdate();

        if (maxTimer - timer > .4f)
        {
            stateMachine.attack.SetActive(false);
        }

        Debug.Log("Attack");

        if (timer <= 0)
        {
            Debug.Log("Done Attacking");
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
        stateMachine.attack.SetActive(false);
    }
}
