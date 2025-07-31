using UnityEngine;

public class HomingArrow : MonoBehaviour
{
    [Header("Tracking")]
    public float speed = 10f;
    public float turnSpeed = 5f;
    public float lifetime = 5f;

    [Header("Damage")]
    public float damage = 15f;

    private Transform target;

    private void Start()
    {
        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                target = playerObj.transform;
            }
        }

        // Destroy arrow after its lifetime
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        if (target == null)
            return;

        // Smoothly rotate toward the target
        Vector3 direction = (target.position - transform.position).normalized;
        Vector3 newDir = Vector3.Lerp(transform.forward, direction, turnSpeed * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(newDir);

        // Move forward
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStateMachine pm = other.GetComponent<PlayerStateMachine>();
            if (pm != null)
            {
                pm.PlayerTakeDamage(damage);
                Debug.Log("Homing arrow hit player.");
            }

            Destroy(gameObject);
        }
        else if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}
