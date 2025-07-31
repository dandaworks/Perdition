using UnityEngine;

public class FinishTutorial : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            PersistentData.beatTutorial = true;
            Debug.Log("Beat tutorial");
        }
    }
}
