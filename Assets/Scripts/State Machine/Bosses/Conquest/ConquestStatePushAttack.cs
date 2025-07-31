using UnityEngine;
using System.Collections;

public class ConquestStatePushAttack : BaseState
{
    ConquestStateMachine stateMachine;

    public ConquestStatePushAttack (BaseStateMachine sm) : base (sm)
    {
        stateMachine = (ConquestStateMachine) sm;
    }

    CharacterController playerController;

    public override void thisStart()
    {
        base.thisStart();

        if (stateMachine.player == null) { stateMachine.ChangeState(stateMachine.stateFollow); return; }

        if (playerController == null)
        {
            if (stateMachine.player != null)
            {
                playerController = stateMachine.player.GetComponent<CharacterController>();
            }
        }

        pushCoroutine = stateMachine.StartCoroutine(Push());
    }

    Coroutine pushCoroutine;

    IEnumerator Push()
    {
        float timerMax = 1.5f;
        float timer = timerMax;

        Vector3 pushDirection = (stateMachine.transform.position - stateMachine.player.position).normalized;

        pushDirection = new Vector3(pushDirection.x, pushDirection.y, 0);

        while (true)
        {
            float currentForce = timer / timerMax;

            //playerController.Move(pushDirection * stateMachine.GetPushForce() * currentForce * Time.deltaTime);

            timer -= Time.deltaTime;
            yield return null;
        }
    }

    public override void thisEnd()
    {
        base.thisEnd();
        stateMachine.StopCoroutine(pushCoroutine);
    }
}
