using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    // PlayButton → MainScene
    public void PlayGame()
    {
        SceneManager.LoadScene("MainScene");
    }

    // CreditsButton → CreditsScene
    public void OpenCredits()
    {
        SceneManager.LoadScene("CreditsScene");
    }

    // GoBack → MainScene
    public void GoBack()
    {
        SceneManager.LoadScene("MenuScene");
    }

    // ExitButton → închide jocul
    public void ExitGame()
    {
        Application.Quit();

        Debug.Log("Game Closed");
    }
}