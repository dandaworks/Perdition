using UnityEngine;

public class BossStateMachine : BaseStateMachine
{
    #region values

    [Header("Navigation")]
    public float detectionRange = 10f;
    public Transform player;
    private UnityEngine.AI.NavMeshAgent agent;
    public UnityEngine.AI.NavMeshAgent GetAgent() { return agent; }

    [Header("Combat")]
    public float contactDamage = 10f;
    public float maxHealth = 50f;
    private float currentHealth;
    private bool isDead = false;

    [Header("Damage Cooldown")]
    private float damageCooldown = 1f; // Time between damage ticks (seconds)
    private float lastDamageTime = -999f;

    #endregion


    #region states

    public BossStateIdle stateIdle;

    public override BaseState InitialState()
    {
        return stateIdle;
    }

    public BossStateFollow stateFollow;
    public BossStateDead stateDead;

    public virtual void InstantiateStates() // this can be overridden in derivative classes to add new states
    {
        stateIdle = new BossStateIdle(this);
        stateFollow = new BossStateFollow(this);
        stateDead = new BossStateDead(this);
    }

    #endregion 


    public override void StartFunctions()
    {
        currentHealth = maxHealth;
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
            if (foundPlayer != null)
                player = foundPlayer.transform;
        }

        InstantiateStates();
    }

    public virtual void ChooseAttack() // this can be overridden in derivative classes to add unique parameters and attack options
    { }

    private void OnTriggerStay(Collider other)
    {
        if (isDead) return;

        if (other.CompareTag("Player"))
        {
            if (Time.time - lastDamageTime >= damageCooldown)
            {
                PlayerMovement pm = other.GetComponent<PlayerMovement>();
                if (pm != null)
                {
                    pm.PlayerTakeDamage(contactDamage);
                    Debug.Log("Damage Dealt");
                    lastDamageTime = Time.time;
                }
            }
        }
    }

    public void TakeDamage(float amount)
    {
        if (currentHealth <= 0) return;

        currentHealth -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. Remaining health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        ChangeState(stateDead);
        SetCanChangeStates(false);
    }
}
