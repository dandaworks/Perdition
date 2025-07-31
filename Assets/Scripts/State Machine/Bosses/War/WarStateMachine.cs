using UnityEngine;

public class WarStateMachine : BossStateMachine
{
    [SerializeField] private Animator bossAnimator;
    public Animator GetBossAnimator() => bossAnimator;

    [Header("War Boss Specific")]
    [SerializeField] private GameObject floatingSword;
    public GameObject GetFloatingSword() => floatingSword;

    [SerializeField] private GameObject arrowPrefab;
    public GameObject GetArrowPrefab() => arrowPrefab;

    [SerializeField] private float dashForce = 15f;
    public float GetDashForce() => dashForce;

    // Attack States
    WarStateLongRangeSword stateLongRangeSword;
    WarStateDashSlash stateDashSlash;
    WarStateArrowVolley stateArrowVolley;
    WarStateFlurryStrikes stateFlurryStrikes;

    public override void InstantiateStates()
    {
        base.InstantiateStates();

        stateLongRangeSword = new WarStateLongRangeSword(this);
        stateDashSlash = new WarStateDashSlash(this);
        stateArrowVolley = new WarStateArrowVolley(this);
        stateFlurryStrikes = new WarStateFlurryStrikes(this);
    }

    public override void ChooseAttack()
    {
        base.ChooseAttack();

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > detectionRange * 0.75f)
        {
            // Use floating sword if the player is far
            ChangeState(stateLongRangeSword);
        }
        else
        {
            // Start with a dash and chain into either follow-up
            ChangeState(stateDashSlash);
        }
    }

    public void ChainToArrowVolley()
    {
        ChangeState(stateArrowVolley);
    }

    public void ChainToFlurryStrikes()
    {
        ChangeState(stateFlurryStrikes);
    }
}
