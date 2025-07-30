using UnityEngine;

public class FamineStateHeadAttack : BaseState
{
    FamineStateMachine stateMachine;
    public FamineStateHeadAttack(BaseStateMachine sm) : base(sm)
    {
        stateMachine = (FamineStateMachine)sm;
    }

    GameObject thisReference; // stored copy of enemy head object
    float timer;

    public override void thisStart()
    {
        base.thisStart();

        timer = 5f;
        thisReference = Object.Instantiate(stateMachine.GetHeadAttackObject(), stateMachine.transform.position, Quaternion.identity);
    }

    public override void thisUpdate()
    {
        base.thisUpdate();

        if (timer <= 0) { if (thisReference) { Object.Destroy(thisReference); } } else { timer -= Time.deltaTime; }

        if (thisReference == null) { stateMachine.ChangeState(stateMachine.stateFollow); }
    }
}
