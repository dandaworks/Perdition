using UnityEngine;

public class HurtPlayerOnTrigger : MonoBehaviour
{
    bool hasHurtPlayer;
    [SerializeField] int damage;

    private void OnEnable()
    {
        hasHurtPlayer = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHurtPlayer) return;

        if (other.CompareTag("Player"))
        {
            PlayerMovement pm = other.GetComponent<PlayerMovement>();
            if (pm != null)
            {
                pm.PlayerTakeDamage(damage);
                hasHurtPlayer = true;
            }
        }
    }
}
