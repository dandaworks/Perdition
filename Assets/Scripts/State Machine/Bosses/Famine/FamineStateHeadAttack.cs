using UnityEngine;

public class FamineStateHeadAttack : BaseState
{
    FamineStateMachine stateMachine;
    public FamineStateHeadAttack(BaseStateMachine sm) : base(sm)
    {
        stateMachine = (FamineStateMachine)sm;
    }

    GameObject thisReference; // stored copy of enemy head object

    bool objectSummoned;
    float timer;
    float maxTimer = 8f;

    public override void thisStart()
    {
        base.thisStart();

        timer = maxTimer;
        objectSummoned = false;

        stateMachine.animator.Play("HeadAttack");
    }

    public override void thisUpdate()
    {
        base.thisUpdate();

        if (!objectSummoned && (maxTimer - timer) >= .66f) 
        {
            objectSummoned = true;
            thisReference = Object.Instantiate(stateMachine.GetHeadAttackObject(), stateMachine.headposition.position, Quaternion.identity);
            thisReference.GetComponent<FamineHeadFollow>()?.SetTarget(stateMachine.player);
        }

        if (timer <= 0) { if (thisReference) { Object.Destroy(thisReference); } } else { timer -= Time.deltaTime; }

        if (objectSummoned && thisReference == null) { stateMachine.ChangeState(stateMachine.stateFollow); }
    }
}
