using UnityEngine;

public class FamineStateMachine : BossStateMachine
{
    public FamineStateClawAttack stateClawAttack;
    public FamineStateHeadAttack stateHeadAttack;

    [Header("Attack Objects")]

    [SerializeField] GameObject headAttackObj;
    public GameObject GetHeadAttackObject() { return headAttackObj; }

    public Transform headposition;
    public GameObject handAttack;

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
