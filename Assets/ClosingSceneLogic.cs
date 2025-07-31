using UnityEngine;
using UnityEngine.SceneManagement;

public class ClosingSceneLogic : MonoBehaviour
{
    float timer = 4f;

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            SceneManager.LoadScene(0);
        }
    }
}
