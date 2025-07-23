using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSwordCombat : MonoBehaviour
{
    [Header("Combat Settings")]
    public Animator animator;
    public float damageAmount = 10f;
    public string targetTag = "Enemy";

    private PlayerControls controls;
    private bool isAttacking = false;

    private void Awake()
    {
        controls = new PlayerControls();
        controls.Player.GreatSword.performed += ctx => Attack();
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void Attack()
    {
        animator.SetTrigger("Attack");
    }

    // These get called from animation events
    public void EnableDamage()
    {
        isAttacking = true;
        Debug.Log("Damage ON");
    }

    public void DisableDamage()
    {
        isAttacking = false;
        Debug.Log("Damage OFF");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger hit: " + other.name);

        if (!isAttacking) return;

        if (other.CompareTag(targetTag))
        {
            Debug.Log("Tag is Enemy.");
            var enemy = other.GetComponent<BasicEnemy>();
            if (enemy != null)
            {
                Debug.Log("Applying damage.");
                enemy.TakeDamage(damageAmount);
            }
        }
    }
}