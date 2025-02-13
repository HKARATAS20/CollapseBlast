using TMPro;
using UnityEngine;

using UnityEngine.UI;

public class ErrorPanel : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text errorMessageText;
    [SerializeField] private Button closeButton;

    private void Start()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseErrorPanel);
        }
        HideErrorPanel();
    }

    public void DisplayError(string message)
    {
        if (errorMessageText != null)
        {
            errorMessageText.text = message;
        }

        if (panel != null)
        {
            panel.SetActive(true);
        }
    }


    public void CloseErrorPanel()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }

        if (errorMessageText != null)
        {
            errorMessageText.text = "";
        }
    }

    private void HideErrorPanel()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }

        if (errorMessageText != null)
        {
            errorMessageText.text = "";
        }
    }
}
