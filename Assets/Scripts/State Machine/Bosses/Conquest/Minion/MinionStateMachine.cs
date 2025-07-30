using UnityEngine;

public class MinionStateMachine : BossStateMachine
{
    public MinionStateAttack stateAttack;

    public override void InstantiateStates()
    {
        base.InstantiateStates();
        stateAttack = new MinionStateAttack(this);
    }
}
