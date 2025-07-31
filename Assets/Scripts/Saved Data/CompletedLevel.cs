using UnityEngine;

public class CompletedLevel : MonoBehaviour
{
    [SerializeField] int levelNumber;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (levelNumber == 0 && !PersistentData.defeatedConquest)
        {
            gameObject.SetActive(false);
        }

        if (levelNumber == 1 && !PersistentData.defeatedWar)
        {
            gameObject.SetActive(false);
        }

        if (levelNumber == 2 && !PersistentData.defeatedFamine)
        {
            gameObject.SetActive(false);
        }
    }
}
