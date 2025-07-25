using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public string playerTag = "Player";
    public bool onCollide = true;
    public bool chooseScene = false;
    public int chooseSceneIndex = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnTriggerEnter(Collider other)
    {
        if (onCollide && other.CompareTag(playerTag))
        {
            LoadScene();
        }
    }
    public void LoadScene()
    {
        if (!chooseScene)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            SceneManager.LoadScene(chooseSceneIndex);
        }

    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
