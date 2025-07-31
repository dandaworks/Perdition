using UnityEngine;

public class PlayerStateMeleeLight : BaseState
{
    PlayerStateMachine stateMachine;

    public PlayerStateMeleeLight(PlayerStateMachine sm) : base(sm)
    {
        stateMachine = sm;
    }

    float timer;

    public override void thisStart()
    {
        base.thisStart();

        stateMachine.propSpear.SetActive(false);
        stateMachine.weaponSpear.SetActive(true);

        stateMachine.animator.Play("LightAttack");

        timer = .2f;
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


        stateMachine.propSpear.SetActive(true);
        stateMachine.weaponSpear.SetActive(false);

        stateMachine.SetCanUseStamina(true);
    }
}
