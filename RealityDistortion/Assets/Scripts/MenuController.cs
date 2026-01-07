using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void OpenCredits()
    {
        SceneManager.LoadScene("CreditsScene");
    }

    public void GoBack()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void OpenDescription()
    {
        SceneManager.LoadScene("DescriptionScene");
    }

    public void ExitGame()
    {
        Application.Quit();

        Debug.Log("Game Closed");
    }
}