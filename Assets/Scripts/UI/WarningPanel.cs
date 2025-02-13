using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class JokingWarningPanel : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text titleText;
    public TMP_Text bodyText;
    public Button affirmativeButton;
    public Button cancelButton;

    private int clickCounter = 0;


    private readonly string[] titleMessages = new string[]
    {
        "Warning!",
        "Hold on...",
        "Uh-oh!",
        "Are you sure?",
    };

    private readonly string[] bodyMessages = new string[]
    {
        "The following screen is created for test purposes enter at your own risk",
        "Are you sure you want to continue",
        "This might be a bad idea, but sure, go ahead!",
    };

    private void Start()
    {
        UpdateWarningMessages();

        affirmativeButton.onClick.AddListener(OnAffirmativeClick);
        cancelButton.onClick.AddListener(OnCancelClick);
    }

    private void OnAffirmativeClick()
    {
        clickCounter++;

        if (clickCounter > 2)
        {
            SceneManager.LoadScene(1);
        }
        else
        {
            UpdateWarningMessages();
        }
    }

    private void OnCancelClick()
    {
        gameObject.SetActive(false);
    }

    private void UpdateWarningMessages()
    {
        titleText.text = titleMessages[clickCounter % titleMessages.Length];
        bodyText.text = bodyMessages[clickCounter % bodyMessages.Length];
    }
}
