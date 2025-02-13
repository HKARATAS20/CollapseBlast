using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    private BlastablePool pool;
    private MatchableGrid grid;

    [SerializeField] GameObject levelClearedPanel;
    [SerializeField] GameObject levelFailedPanel;

    Vector2Int dimensions = Vector2Int.one;
    private void Start()
    {
        Application.targetFrameRate = 100;
        if (!ParameterData.Instance.isEndlessMode)
        {
            levelClearedPanel.SetActive(false);
            levelFailedPanel.SetActive(false);
        }

        StartCoroutine(Setup());
    }

    private IEnumerator Setup()
    {

        SetPoolAndGrid();

        //set the camera size based on how many blastalbes we want to fit in the scene
        Camera.main.orthographicSize = math.max(dimensions.x, dimensions.y) + 2;

        yield return null;

        yield return StartCoroutine(grid.PopulateGrid(true));

    }

    private void SetPoolAndGrid()
    {
        dimensions.x = ParameterData.Instance.rows;
        dimensions.y = ParameterData.Instance.columns;

        PoolSetup();
        GridSetup();
        grid.OnGameOver += HandleGameOver;
        grid.OnLevelComplete += HandleLevelComplete;

    }


    private void PoolSetup()
    {
        pool = (BlastablePool)BlastablePool.Instance;

        pool.gameObject.transform.position = new Vector2(-(dimensions.x - 1) / 2f, (-(dimensions.y - 1) / 2f) - 2);

        //give number of colors and pool the necessary amount of blastables
        pool.HowManyTypes = ParameterData.Instance.colors;
        pool.PoolObjects(dimensions.x * dimensions.y * 2);
    }

    private void GridSetup()
    {

        grid = (MatchableGrid)MatchableGrid.Instance;

        //set the positions of pool and grid on the scene
        grid.gameObject.transform.position = new Vector2(-(dimensions.x - 1) / 2f, (-(dimensions.y - 1) / 2f) - 2);

        //initialize and set the values of the grid
        grid.InitializeGrid(dimensions);
        grid.SetValues(ParameterData.Instance.a, ParameterData.Instance.b, ParameterData.Instance.c,
                        ParameterData.Instance.colors, ParameterData.Instance.currentLevel.maxMoves, ParameterData.Instance.isEndlessMode);
    }

    public void MenuButton()
    {
        SceneManager.LoadScene(0);
    }

    public void ShuffleButton()
    {
        StartCoroutine(grid.Shuffle());
    }

    private void HandleGameOver()
    {
        levelFailedPanel.SetActive(true);
    }


    private void HandleLevelComplete()
    {
        levelClearedPanel.SetActive(true);
    }

    public void ContinueButton()
    {
        SceneManager.LoadScene(0);
    }
}
