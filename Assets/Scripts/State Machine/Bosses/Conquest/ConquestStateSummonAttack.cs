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
        NavMeshHit hit;

        if (NavMesh.SamplePosition(new Vector3(
            Random.Range(stateMachine.GetSpawnLocation().x - stateMachine.GetSpawnSize().x, stateMachine.GetSpawnLocation().x + stateMachine.GetSpawnSize().x),
            stateMachine.GetSpawnLocation().y,
            Random.Range(stateMachine.GetSpawnLocation().z - stateMachine.GetSpawnSize().z, stateMachine.GetSpawnLocation().z + stateMachine.GetSpawnSize().z)),
            out hit, 10f, NavMesh.AllAreas))
        {
            GameObject a = Object.Instantiate(stateMachine.GetConquestSummon(), hit.position, Quaternion.identity);
            stateMachine.GetSummonedGameObjectsList().Add(a);
        }
    }
}
