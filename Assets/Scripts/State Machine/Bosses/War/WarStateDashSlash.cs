using UnityEngine;

public class WarStateDashSlash : BaseState
{
    private WarStateMachine stateMachine;

    private float dashDuration = 1.2f;
    private float dashSpeed = 10f;
    private float timer;

    private Animator bossAnimator;
    private Animator swordAnimator;

    public WarStateDashSlash(BaseStateMachine sm) : base(sm)
    {
        stateMachine = (WarStateMachine)sm;
    }

    public override void thisStart()
    {
        base.thisStart();

        timer = dashDuration;

        stateMachine.GetAgent().isStopped = true;

        // Play boss dash animation
        bossAnimator = stateMachine.GetBossAnimator();
        if (bossAnimator != null)
        {
            bossAnimator.SetTrigger("DashSlash");
        }

        // Play sword swing animation
        if (stateMachine.GetFloatingSword() != null)
        {
            swordAnimator = stateMachine.GetFloatingSword().GetComponent<Animator>();
            if (swordAnimator != null)
            {
                swordAnimator.SetTrigger("SwordSwing");
            }

            // Set the hitbox attack type
            var hitbox = swordAnimator.GetComponent<SwordHitboxController>();
            if (hitbox != null)
            {
                hitbox.EnableDashHitbox(); // optional: call via animation event instead
            }
        }

        Debug.Log("War: Started dash slash attack.");
    }

    public override void thisUpdate()
    {
        base.thisUpdate();

        timer -= Time.deltaTime;

        // Dash forward toward the player
        if (timer > 0.4f && stateMachine.player != null)
        {
            Vector3 direction = (stateMachine.player.position - stateMachine.transform.position).normalized;
            direction.y = 0;
            stateMachine.transform.position += direction * dashSpeed * Time.deltaTime;
        }

        // Chain into a follow-up attack
        if (timer <= 0f)
        {
            float roll = Random.value;
            if (roll < 0.5f)
            {
                stateMachine.ChainToArrowVolley();
                Debug.Log("War: Chaining to ArrowVolley.");
            }
            else
            {
                stateMachine.ChainToFlurryStrikes();
                Debug.Log("War: Chaining to FlurryStrikes.");
            }
        }
    }

    public override void thisEnd()
    {
        base.thisEnd();
        stateMachine.GetAgent().isStopped = false;
    }
}
