using UnityEngine;
using UnityEngine.AI;

public class BasicEnemy : MonoBehaviour
{
  
    [Header("Navigation")]
    public float detectionRange = 10f;
    public Transform player;
    private NavMeshAgent agent;

    [Header("Combat")]
    public float contactDamage = 10f;
    public float maxHealth = 50f;
    private float currentHealth;
    private bool isDead = false;

    [Header("Damage Cooldown")]
    private float damageCooldown = 1f; // Time between damage ticks (seconds)
    private float lastDamageTime = -999f;

    [Header("AI State")]
    private bool hasAgroed = false;

    void Start()
    {
        currentHealth = maxHealth;
        //agent = GetComponent<NavMeshAgent>();

        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
            if (foundPlayer != null)
                player = foundPlayer.transform;
        }
    }

    void Update()
    {
        if (isDead || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (!hasAgroed && distance <= detectionRange)
        {
            hasAgroed = true;
            Debug.Log("Enemy agroed");
        }

        if (hasAgroed)
        {
            agent.SetDestination(player.position);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (isDead) return;

        if (other.CompareTag("Player"))
        {
            if (Time.time - lastDamageTime >= damageCooldown)
            {
                PlayerStateMachine pm = other.GetComponent<PlayerStateMachine>();
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
        if (isDead) return;

        currentHealth -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. Remaining health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log($"{gameObject.name} has died!");

        // Disable movement and collider
        //agent.enabled = false;
        GetComponent<Collider>().enabled = false;

        // Optional: Destroy after delay
        Destroy(gameObject);
    }
}
