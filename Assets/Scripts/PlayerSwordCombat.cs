using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSwordCombat : MonoBehaviour
{
    [Header("Combat Settings")]
    public Animator animator;
    public float damageAmount = 10f;
    public string targetTag = "Enemy";
    public float attackDuration = 0.3f; // Set to match animation length

    private PlayerControls controls;
    private bool isAttacking = false;

    private void Awake()
    {
        controls = new PlayerControls();
        controls.Player.GreatSword.performed += ctx => TryAttack();
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void TryAttack()
    {
        if (isAttacking) return;

        StartCoroutine(AttackRoutine());
    }

    private System.Collections.IEnumerator AttackRoutine()
    {
        isAttacking = true;

        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(attackDuration); 

        isAttacking = false;
    }

    public void EnableDamage()
    {
        Debug.Log("Damage ON");
    }

    public void DisableDamage()
    {
        Debug.Log("Damage OFF");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isAttacking) return;

        Debug.Log("Trigger hit: " + other.name);

        if (other.CompareTag(targetTag))
        {
            Debug.Log("Tag is Enemy.");

            var enemy = other.GetComponent<BasicEnemy>();
            if (enemy != null)
            {
                Debug.Log("Applying damage.");
                enemy.TakeDamage(damageAmount);
                return;
            }

            var boss = other.GetComponent<BossStateMachine>();
            if (boss != null)
            {
                Debug.Log("Applying damage to Boss.");
                boss.TakeDamage(damageAmount);
            }
        }
    }
}