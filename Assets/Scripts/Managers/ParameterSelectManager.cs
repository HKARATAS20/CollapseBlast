
using System;
using TMPro;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ParameterSelectManager : MonoBehaviour
{
    public TMP_InputField rowsInput;
    public TMP_InputField columnsInput;
    public TMP_InputField colorsInput;
    public TMP_InputField aInput;
    public TMP_InputField bInput;
    public TMP_InputField cInput;
    public Button submitButton;

    public ErrorPanel errorPanel;

    public int rows, columns, colors, a, b, c;

    public void OnSubmit()
    {
        string errorMessageText = "";

        if (!int.TryParse(rowsInput.text, out rows) || rows < 2 || rows > 10)
        {
            rows = 0;
            errorMessageText += "Rows (M) must be a  between 2 and 10.\n";
        }

        if (!int.TryParse(columnsInput.text, out columns) || columns < 2 || columns > 10)
        {
            columns = 0;
            errorMessageText += "Columns (N) must be a  between 2 and 10.\n";
        }

        if (!int.TryParse(colorsInput.text, out colors) || colors >= 7)
        {
            colors = 0;
            errorMessageText += "Colors (K) must be less than 7.\n";
        }

        if (!int.TryParse(aInput.text, out a))
        {
            a = 0;
            errorMessageText += "A must be a valid integer.\n";
        }

        if (!int.TryParse(bInput.text, out b))
        {
            b = 0;
            errorMessageText += "B must be a valid integer.\n";
        }

        if (!int.TryParse(cInput.text, out c))
        {
            c = 0;
            errorMessageText += "C must be a valid integer.\n";
        }
        if (a >= b || a >= c || b >= c)
        {
            errorMessageText += "For icons to work properly, C > B > A must hold.\n";
        }

        if (!string.IsNullOrEmpty(errorMessageText))
        {
            errorPanel.DisplayError(errorMessageText.TrimEnd('\n'));
            return;
        }

        ParameterData.Instance.rows = rows;
        ParameterData.Instance.columns = columns;
        ParameterData.Instance.colors = colors;
        ParameterData.Instance.a = a;
        ParameterData.Instance.b = b;
        ParameterData.Instance.c = c;
        ParameterData.Instance.isEndlessMode = true;

        SceneManager.LoadScene(3);
    }

    public void BackButton()
    {
        SceneManager.LoadScene(0);
    }

}
