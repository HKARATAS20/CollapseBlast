using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CharacterBarManager : MonoBehaviour
{
    [SerializeField] private TMP_Text movesText;
    [SerializeField] private TMP_Text objectiveText;
    [SerializeField] private GameObject characterBar;
    [SerializeField] private TMP_Text runestoneText;
    [SerializeField] private TMP_Text chestText;
    [SerializeField] private GameObject twoObjectsContainer;
    [SerializeField] private GameObject oneObjectContainer;
    [SerializeField] private Image oneObjectImage;
    [SerializeField] private Sprite runestone;
    [SerializeField] private Sprite chest;

    public void SetCharacterBarActive(bool isActive)
    {
        characterBar.SetActive(isActive);
    }

    public void UpdateMovesText(int moves)
    {
        movesText.text = moves.ToString();
    }

    public void UpdateObstacleDisplay(bool hasTwoObstacles, int[] obstacleCounts)
    {
        int runestoneCount = obstacleCounts[0];
        int chestCount = obstacleCounts[1];
        if (hasTwoObstacles)
        {
            twoObjectsContainer.SetActive(true);
            oneObjectContainer.SetActive(false);
            runestoneText.text = runestoneCount.ToString();
            chestText.text = chestCount.ToString();
        }
        else
        {
            twoObjectsContainer.SetActive(false);
            oneObjectContainer.SetActive(true);
            oneObjectImage.sprite = runestoneCount > 0 ? runestone : chest;
            objectiveText.text = (runestoneCount > 0 ? runestoneCount : chestCount).ToString();
        }
    }
}