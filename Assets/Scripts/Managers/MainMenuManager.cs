using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Elements")]

    public Button levelsButton;
    public Button parameterButton;
    public Button backButton;
    public Button endlessModeButton;
    public GameObject warningPanel;
    public GameObject levelsPanel;

    public void ParameterButton()
    {
        warningPanel.SetActive(true);
    }

    public void EndlessModeButton()
    {
        ParameterData.Instance.LoadLevel(-1);
        SceneManager.LoadScene(3);
    }

    public void LoadLevel(int level)
    {
        ParameterData.Instance.LoadLevel(level);
        SceneManager.LoadScene(2);
    }

    public void LevelsButton()
    {
        parameterButton.gameObject.SetActive(false);
        levelsButton.gameObject.SetActive(false);
        endlessModeButton.gameObject.SetActive(false);

        levelsPanel.SetActive(true);
        backButton.gameObject.SetActive(true);
    }

    public void BackButton()
    {
        parameterButton.gameObject.SetActive(true);
        levelsButton.gameObject.SetActive(true);
        endlessModeButton.gameObject.SetActive(true);

        levelsPanel.SetActive(false);
        backButton.gameObject.SetActive(false);

    }

}
