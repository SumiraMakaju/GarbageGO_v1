using UnityEngine;
using UnityEngine.SceneManagement;

public class Home : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("MLAgentScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
