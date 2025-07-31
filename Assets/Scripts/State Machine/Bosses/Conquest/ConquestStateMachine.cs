using UnityEngine;
using System.Collections.Generic;

public class ConquestStateMachine : BossStateMachine
{
    public Vector3 IdlePosition;
    public Vector3 StartPosition;

    //[SerializeField] float pushForce;
    //public float GetPushForce() { return pushForce; }

    ConquestStatePushAttack statePushAttack;
    ConquestStateSummonAttack stateSummonAttack;
    ConquestStateSwordAttack stateSwordAttack;
    ConquestStateIdle stateIdle;
    public override BaseState InitialState()
    {
        return stateIdle;
    }

    [Header("Summon Skeleton Minions")]

    public Vector3[] spawnPositions;

    [SerializeField] GameObject conquestSummon;
    public GameObject GetConquestSummon() { return conquestSummon; }
    List<GameObject> summonedGameobjects = new List<GameObject>();
    public List<GameObject> GetSummonedGameObjectsList() { return summonedGameobjects; }

    [Header("Summon Blast")]
    [SerializeField] GameObject summonable;
    public GameObject GetSummonable() { return summonable; }
    [SerializeField] Transform summonPosition;
    public Transform GetSummonPosition() { return summonPosition; }

    public override void InstantiateStates()
    {
        base.InstantiateStates();
        statePushAttack = new ConquestStatePushAttack(this);
        stateSummonAttack = new ConquestStateSummonAttack(this);
        stateSwordAttack = new ConquestStateSwordAttack(this);
        stateIdle = new ConquestStateIdle(this);
    }

    public override void ChooseAttack()
    {
        base.ChooseAttack();

        if (Random.Range(0, 10) < 5)
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

        ChangeState(stateSwordAttack);
    }

    public override void Die()
    {
        PersistentData.defeatedConquest = true;

        base.Die();
    }
}