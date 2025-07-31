using UnityEngine;

public class SwordHitboxController : MonoBehaviour
{
    public enum AttackType
    {
        None,
        LongRange,
        Dash,
        Flurry
    }

    [Header("Hitbox Settings")]
    [SerializeField] private Collider hitbox;

    [Header("Damage Settings")]
    public float longRangeDamage = 15f;
    public float dashDamage = 30f;
    public float flurryDamage = 8f;

    private AttackType currentAttack = AttackType.None;

    private void Start()
    {
        if (hitbox != null)
            hitbox.enabled = false;
    }

    // Called by animation event
    public void EnableLongRangeHitbox()
    {
        currentAttack = AttackType.LongRange;
        EnableHitbox();
    }

    public void EnableDashHitbox()
    {
        currentAttack = AttackType.Dash;
        EnableHitbox();
    }

    public void EnableFlurryHitbox()
    {
        currentAttack = AttackType.Flurry;
        EnableHitbox();
    }

    private void EnableHitbox()
    {
        if (hitbox != null)
            hitbox.enabled = true;
    }

    public void DisableHitbox()
    {
        if (hitbox != null)
            hitbox.enabled = false;

        currentAttack = AttackType.None;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hitbox.enabled) return;

        if (other.CompareTag("Player"))
        {
            PlayerStateMachine pm = other.GetComponent<PlayerStateMachine>();
            if (pm != null)
            {
                DisableHitbox();
                float damage = GetDamageForAttack();
                pm.PlayerTakeDamage(damage);
                Debug.Log($"Sword hit player for {damage} damage ({currentAttack})");
            }
        }
    }

    private float GetDamageForAttack()
    {
        switch (currentAttack)
        {
            case AttackType.LongRange: return longRangeDamage;
            case AttackType.Dash: return dashDamage;
            case AttackType.Flurry: return flurryDamage;
            default: return 0f;
        }
    }
}
