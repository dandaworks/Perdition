using UnityEngine;
using System.Collections.Generic;

public class ConquestStateMachine : BossStateMachine
{
    [SerializeField] float pushForce;
    public float GetPushForce() { return pushForce; }

    ConquestStatePushAttack statePushAttack;
    ConquestStateSummonAttack stateSummonAttack;
    ConquestStateSwordAttack stateSwordAttack;

    [Header("Summon Skeleton Minions")]
    [SerializeField] Vector3 spawnPositionsLocation;
    public Vector3 GetSpawnLocation() { return spawnPositionsLocation; }
    [SerializeField] Vector3 spawnPositionsSize;
    public Vector3 GetSpawnSize() { return spawnPositionsSize; }

    [SerializeField] GameObject conquestSummon;
    public GameObject GetConquestSummon() { return conquestSummon; }
    List<GameObject> summonedGameobjects = new List<GameObject>();
    public List<GameObject> GetSummonedGameObjectsList() { return summonedGameobjects; }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(spawnPositionsLocation, spawnPositionsSize);
    }

    public override void InstantiateStates()
    {
        base.InstantiateStates();
        statePushAttack = new ConquestStatePushAttack(this);
        stateSummonAttack = new ConquestStateSummonAttack(this);
        stateSwordAttack = new ConquestStateSwordAttack(this);
    }

    public override void ChooseAttack()
    {
        base.ChooseAttack();

        if (Random.Range(0, 10) < 3)
        {
            bool test = true;
         
            if (summonedGameobjects.Count != 0)
            {
                for (int i = 0; i < summonedGameobjects.Count; i++)
                {
                    if (summonedGameobjects[i] != null) { test = false; }
                }
            }

            if (test)
            {
                ChangeState(stateSummonAttack);
                return;
            }
        }

        if (Vector3.Distance(transform.position, player.transform.position) < detectionRange * .85f)
        {
            if (Random.Range(0, 10) < 6)
            {
                ChangeState(statePushAttack);
                return;
            }
        }

        ChangeState(stateSwordAttack);
    }
}