using UnityEngine;
using UnityEngine.AI;

public class ConquestStateSummonAttack : BaseState
{
    ConquestStateMachine stateMachine;

    public ConquestStateSummonAttack(BaseStateMachine sm) : base(sm)
    {
        stateMachine = (ConquestStateMachine)sm;
    }

    float timer;

    public override void thisStart()
    {
        base.thisStart();

        SummonSkeleton();
        SummonSkeleton();

        timer = 5f;
    }

    public override void thisUpdate()
    {
        base.thisUpdate();

        timer -= Time.deltaTime;

        if (timer <= 0) { stateMachine.ChangeState(stateMachine.stateFollow); }
    }

    void SummonSkeleton()
    {
        Vector3 randomPos = stateMachine.spawnPositions[Random.Range(0, stateMachine.spawnPositions.Length - 1)];
        GameObject a = Object.Instantiate(stateMachine.GetConquestSummon(), randomPos, Quaternion.identity);
        stateMachine.GetSummonedGameObjectsList().Add(a);
    }
}
