using UnityEngine;

public class MinionStateMachine : BossStateMachine
{
    public MinionStateAttack stateAttack;

    public GameObject attack;

    public override void InstantiateStates()
    {
        base.InstantiateStates();
        stateAttack = new MinionStateAttack(this);
    }

    public override void ChooseAttack()
    {
        base.ChooseAttack();
        if (Random.Range(0, 10) < 20 && Vector3.Distance(transform.position, player.position) < 20f)
        {
            ChangeState(stateAttack);
        }
    }
}
