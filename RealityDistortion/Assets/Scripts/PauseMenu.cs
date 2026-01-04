using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseCanvas;
    private bool isPaused = false;
    public AudioSource pauseMusic;

    void Awake()
    {
        Debug.Log("AWAKE - PauseMenu");
    }

    void Start()
    {
        Debug.Log("START - PauseMenu");

        if (pauseMusic != null)
            pauseMusic.Stop();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    void Pause()
    {
        pauseCanvas.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        if (pauseMusic != null)
        {
            pauseMusic.loop = true;
            pauseMusic.Play();
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        pauseCanvas.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        if (pauseMusic != null)
            pauseMusic.Stop();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuScene");
    }

    public void QuitGame()
    {
        SceneManager.LoadScene("MenuScene");
        Application.Quit();
        Debug.Log("Quit Game");
    }
}
