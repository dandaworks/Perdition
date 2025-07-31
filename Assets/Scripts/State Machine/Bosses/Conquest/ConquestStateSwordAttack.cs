using UnityEngine;
using System.Collections;

public class ConquestStateSwordAttack : BaseState
{
    ConquestStateMachine stateMachine;

    public ConquestStateSwordAttack(BaseStateMachine sm) : base(sm)
    {
        stateMachine = (ConquestStateMachine)sm;
    }

    Coroutine coroutine;

    public override void thisStart()
    {
        base.thisStart();

        coroutine = stateMachine.StartCoroutine(attack());
    }

    IEnumerator attack()
    {
        stateMachine.animator.Play("Pre-Blast");
        yield return new WaitForSeconds(1f);
        stateMachine.animator.Play("Blast");
        yield return new WaitForSeconds(1.4f);
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(.1f);
            SummonAttack();
        }
        yield return new WaitForSeconds(1f);
        stateMachine.ChangeState(stateMachine.stateFollow);
    }

    public override void thisEnd()
    {
        base.thisEnd();

        stateMachine.StopCoroutine(coroutine);
    }

    void SummonAttack()
    {
        GameObject a = Object.Instantiate(stateMachine.GetSummonable(), stateMachine.GetSummonPosition().position, Quaternion.identity);
        a.GetComponent<AimAtAndHurtPlayer>().target = stateMachine.player;
    }
}
