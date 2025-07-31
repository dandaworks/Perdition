using UnityEngine;
using UnityEngine.SceneManagement;

public class HubLogicScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PersistentData.defeatedConquest && PersistentData.defeatedFamine && PersistentData.defeatedWar && !PersistentData.endGame) 
        {
            PersistentData.endGame = true;
            SceneManager.LoadScene(7);
        }   
    }
}
