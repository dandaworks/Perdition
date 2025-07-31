using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using piqey.Utilities.Editor;

public class LevelManager : MonoBehaviour
{
    public string playerTag = "Player";
    public bool onCollide = true;
    public bool chooseScene = false;
    public bool timer;
    public float waitTime = 5f;

    [Tooltip("The scene that should be loaded if ChooseScene is toggled On")]
    public SceneField SceneToLoad;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (timer)
            StartCoroutine(nameof(Wait), waitTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (onCollide && other.CompareTag(playerTag))
            LoadScene();
    }

    IEnumerator Wait(float timer)
    {
        yield return new WaitForSeconds(waitTime);
        LoadScene();
    }

    public void LoadScene()
    {
        if (!chooseScene)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        else
            SceneManager.LoadScene((string)SceneToLoad);
    }

    public void ExitGame() =>
        Application.Quit();
}
