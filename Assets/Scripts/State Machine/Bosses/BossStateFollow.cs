using UnityEngine;
using System.Collections;

public class BossStateFollow : BaseState
{
    BossStateMachine stateMachine;

    public BossStateFollow(BaseStateMachine sm) : base(sm)
    {
        stateMachine = (BossStateMachine)sm;
    }

    Coroutine checkPlayerPositionCoroutine;

    float attackTime;

    public override void thisStart()
    {
        base.thisStart();

        if (stateMachine.animator)
        {
            stateMachine.animator.Play("Walk");
        }

        attackTime = Random.Range(stateMachine.maxTimeBetweenAttacks * .66f, stateMachine.maxTimeBetweenAttacks);

        stateMachine.GetAgent().isStopped = false;
        checkPlayerPositionCoroutine = stateMachine.StartCoroutine(CheckPlayerPosition());
    }

    IEnumerator CheckPlayerPosition()
    {
        while (true)
        {
            stateMachine.GetAgent().SetDestination(stateMachine.player.position);
            yield return new WaitForSeconds(.2f);
        }
    }

    public override void thisUpdate()
    {
        base.thisUpdate();

        attackTime -= Time.deltaTime;

        if (attackTime <= 0)
        {
            stateMachine.ChooseAttack();
        }
    }

    public override void thisEnd()
    {
        base.thisEnd();

        stateMachine.StopCoroutine(checkPlayerPositionCoroutine);
        stateMachine.GetAgent().isStopped = true;
    }
}
