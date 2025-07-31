using UnityEngine;

public class WarStateArrowVolley : BaseState
{
    private WarStateMachine stateMachine;

    private float volleyDuration = 2f;
    private float timer;
    private bool hasFired = false;

    private Animator bossAnimator;

    public WarStateArrowVolley(BaseStateMachine sm) : base(sm)
    {
        stateMachine = (WarStateMachine)sm;
    }

    public override void thisStart()
    {
        base.thisStart();

        timer = volleyDuration;
        hasFired = false;

        // Stop movement
        stateMachine.GetAgent().isStopped = true;

        // Trigger volley animation
        bossAnimator = stateMachine.GetBossAnimator();
        if (bossAnimator != null)
        {
            bossAnimator.SetTrigger("ArrowVolley");
        }

        Debug.Log("War: Started arrow volley.");
    }

    public override void thisUpdate()
    {
        base.thisUpdate();

        timer -= Time.deltaTime;

        if (!hasFired && timer <= volleyDuration * 0.5f)
        {
            hasFired = true;
            FireHomingArrow();
        }

        if (timer <= 0f)
        {
            stateMachine.ChangeState(stateMachine.stateFollow);
        }
    }

    private void FireHomingArrow()
    {
        GameObject arrowPrefab = stateMachine.GetArrowPrefab();
        if (arrowPrefab == null || stateMachine.player == null)
        {
            Debug.LogWarning("Missing arrow prefab or player reference.");
            return;
        }

        Vector3 spawnPos = stateMachine.transform.position + Vector3.up * 1.5f + stateMachine.transform.forward * 1.0f;
        GameObject arrow = Object.Instantiate(arrowPrefab, spawnPos, Quaternion.identity);

        Debug.Log("War: Fired a homing arrow.");
    }

    public override void thisEnd()
    {
        base.thisEnd();
        stateMachine.GetAgent().isStopped = false;
    }
}
