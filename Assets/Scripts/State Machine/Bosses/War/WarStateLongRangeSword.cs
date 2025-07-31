using UnityEngine;

public class WarStateLongRangeSword : BaseState
{
    private WarStateMachine stateMachine;

    private float attackDuration = 3f;
    private float timer;

    private Animator swordAnimator;
    private Animator bossAnimator;

    public WarStateLongRangeSword(BaseStateMachine sm) : base(sm)
    {
        stateMachine = (WarStateMachine)sm;
    }

    public override void thisStart()
    {
        base.thisStart();

        timer = attackDuration;

        stateMachine.GetAgent().isStopped = true;

        // Trigger boss animation
        bossAnimator = stateMachine.GetBossAnimator();
        if (bossAnimator != null)
        {
            bossAnimator.SetTrigger("CastSword");
        }

        // Trigger sword animation
        if (stateMachine.GetFloatingSword() != null)
        {
            swordAnimator = stateMachine.GetFloatingSword().GetComponent<Animator>();
            if (swordAnimator != null)
            {
                swordAnimator.SetTrigger("SwordAttack");
            }
        }

        Debug.Log("War: Starting long-range sword attack.");
    }

    public override void thisUpdate()
    {
        base.thisUpdate();

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            stateMachine.ChangeState(stateMachine.stateFollow);
        }
    }

    public override void thisEnd()
    {
        base.thisEnd();
        stateMachine.GetAgent().isStopped = false;
    }
}
