using UnityEngine;

public class BossStateDead : BaseState
{
    BossStateMachine stateMachine;

    public BossStateDead(BaseStateMachine sm) : base(sm)
    {
        stateMachine = (BossStateMachine)sm;
    }

    public override void thisStart()
    {
        base.thisStart();

        if (stateMachine.animator) { stateMachine.animator.Play("Dead"); }

        //Debug.Log($"{stateMachine.gameObject.name} has died!");
        stateMachine.GetComponent<Collider>().enabled = false;
        // Optional: Destroy after delay
        Object.Destroy(stateMachine.gameObject, 1f);
    }
}
