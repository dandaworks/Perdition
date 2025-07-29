using UnityEngine;

public class FamineStateMachine : BossStateMachine
{
    public FamineStateClawAttack stateClawAttack;
    public FamineStateHeadAttack stateHeadAttack;

    [SerializeField] GameObject headAttackObj;
    public GameObject GetHeadAttackObject() { return headAttackObj; }

    public override void InstantiateStates()
    {
        base.InstantiateStates();
        stateClawAttack = new FamineStateClawAttack(this);
        stateHeadAttack = new FamineStateHeadAttack(this);
    }

    public override void ChooseAttack()
    {
        base.ChooseAttack();

        if (Vector3.Distance(player.transform.position, transform.position) <= detectionRange * .75f)
        {
            ChangeState(stateClawAttack);
        }
        else
        {
            ChangeState(stateHeadAttack);
        }
    }
}
