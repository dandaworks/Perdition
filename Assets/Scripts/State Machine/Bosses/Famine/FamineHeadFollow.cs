using UnityEngine;

public class FamineHeadFollow : MonoBehaviour
{
    [SerializeField] Transform target;

    public void SetTarget(Transform a) { target = a; }
    [SerializeField] float speed;
    [SerializeField] int damage;

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        transform.LookAt(target);
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
