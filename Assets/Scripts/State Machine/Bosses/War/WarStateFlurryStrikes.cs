using UnityEngine;

public class WarStateFlurryStrikes : BaseState
{
    private WarStateMachine stateMachine;

    private float flurryDuration = 2.5f;
    private float timer;

    private Animator bossAnimator;
    private Animator swordAnimator;

    public WarStateFlurryStrikes(BaseStateMachine sm) : base(sm)
    {
        stateMachine = (WarStateMachine)sm;
    }

    public override void thisStart()
    {
        base.thisStart();

        timer = flurryDuration;

        // Stop movement
        stateMachine.GetAgent().isStopped = true;

        // Play boss flurry animation
        bossAnimator = stateMachine.GetBossAnimator();
        if (bossAnimator != null)
        {
            bossAnimator.SetTrigger("FlurryStrikes");
        }

        // Play sword flurry animation
        if (stateMachine.GetFloatingSword() != null)
        {
            swordAnimator = stateMachine.GetFloatingSword().GetComponent<Animator>();
            if (swordAnimator != null)
            {
                swordAnimator.SetTrigger("SwordFlurry");
            }
        }

        Debug.Log("War: Started flurry of strikes.");
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
