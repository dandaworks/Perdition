using UnityEngine;

public class PlayerStateDash : BaseState
{
    PlayerStateMachine stateMachine;

    public PlayerStateDash (PlayerStateMachine sm) : base(sm)
    {
        stateMachine = sm;
    }

    public override void thisStart()
    {
        base.thisStart();
        stateMachine.animator.Play("Dash");
        stateMachine.audioSource.PlayOneShot(stateMachine.dash);

        stateMachine.StartCoroutine(DashRoutine());
    }

    System.Collections.IEnumerator DashRoutine()
    {
        stateMachine.SetCanDash(false);

        Vector3 dashDirection = stateMachine.transform.forward;
        float elapsed = 0f;

        while (elapsed < stateMachine.dashDuration)
        {
            stateMachine.GetController().Move(stateMachine.dashSpeed * Time.deltaTime * dashDirection);
            elapsed += Time.deltaTime;
            yield return null;
        }

        stateMachine.ChangeState(stateMachine.InitialState());

        yield return new WaitForSeconds(stateMachine.dashCooldown);
        stateMachine.SetCanDash(true);
    }

    public override void thisEnd()
    {
        base.thisEnd();

        stateMachine.SetCanUseStamina(true);
    }
}
