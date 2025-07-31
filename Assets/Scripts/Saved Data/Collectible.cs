using UnityEngine;

public class Collectible : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PersistentData.collectibleNames.Contains(gameObject.name)) { Destroy(gameObject); }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            PersistentData.collectibleNames.Add(gameObject.name);
            Destroy(gameObject);
        }
    }
}
