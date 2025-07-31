using UnityEngine;

public class PlayerStateMeleeStrong : BaseState
{
    PlayerStateMachine stateMachine;

    public PlayerStateMeleeStrong(PlayerStateMachine sm) : base(sm)
    {
        stateMachine = sm;
    }

    float timer;

    public override void thisStart()
    {
        base.thisStart();

        stateMachine.audioSource.PlayOneShot(stateMachine.heavyAttack);
        stateMachine.propSword.SetActive(false);
        stateMachine.weaponSword.SetActive(true);

        stateMachine.animator.Play("HeavyAttack");

        timer = 1f;
    }

    public override void thisUpdate()
    {
        base.thisUpdate();

        timer -= Time.deltaTime;

        if (timer <= 0) { stateMachine.ChangeState(stateMachine.InitialState()); }
    }

    public override void thisEnd()
    {
        base.thisEnd();

        stateMachine.propSword.SetActive(true);
        stateMachine.weaponSword.SetActive(false);

        stateMachine.SetCanUseStamina(true);
    }
}
