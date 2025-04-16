using UnityEngine;
using UnityEngine.SceneManagement;
public class TutorialButton : MonoBehaviour
{
    [SerializeField] Canvas MainMenu;
    [SerializeField] Canvas Tutorial;
    public void GoToMainMenu()
    {
        MainMenu.gameObject.SetActive(true);
        Tutorial.gameObject.SetActive(false);
    }

    public void GoToTutorial()
    {
        MainMenu.gameObject.SetActive(false);
        Tutorial.gameObject.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
