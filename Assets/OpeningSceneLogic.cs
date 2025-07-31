using UnityEngine;
using UnityEngine.SceneManagement;

public class OpeningSceneLogic : MonoBehaviour
{
    float timer = 4f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (PersistentData.beatTutorial) { SceneManager.LoadScene(3); }
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            SceneManager.LoadScene(2);
        }
    }
}
