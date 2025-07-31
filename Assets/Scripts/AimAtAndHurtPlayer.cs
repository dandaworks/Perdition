using UnityEngine;

public class AimAtAndHurtPlayer : MonoBehaviour
{
    public Transform target;
    public float speed;
    public int damage;

    public float timeToDestroy = 10f;

    Vector3 movementDirection;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.LookAt(target);

        movementDirection = (target.position - transform.position).normalized;

        Destroy(gameObject, timeToDestroy);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += movementDirection * speed * Time.deltaTime;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == target.gameObject)
        {
            PlayerMovement player = target.gameObject.GetComponent<PlayerMovement>();
            if (player)
            {
                player.PlayerTakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}
