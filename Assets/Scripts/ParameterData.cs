using System.Collections.Generic;
using UnityEngine;

public class ParameterData : MonoBehaviour
{
    public static ParameterData Instance;

    public int rows, columns, colors, a, b, c;
    public LevelData currentLevel;

    public bool isEndlessMode = false;

    public List<LevelData> levels = new();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeLevels(); // Load predefined level data
    }

    private void InitializeLevels()
    {
        levels = new List<LevelData>
        {
            new(7, 7, 4, 3, 5, 7, -1, new List<Vector3Int> { }),
            new(6, 6, 3, 3, 5, 7, 15, new List<Vector3Int> { new(3, 2,1), new(2, 4,1), new(5, 5,0) }),
            new(7, 7, 3, 3, 5, 7, 18, new List<Vector3Int> {new(0, 0,1),new(1, 1,1),new(2, 2,1),new(3, 3,1), new(4, 4,1), new(5, 5,1), new(6, 6,1) }),
            new(8, 8, 4, 3, 5, 7, 30, new List<Vector3Int> { new(3, 0,0), new(3, 1,0), new(3, 2,0),new(3, 3,0),new(3, 4,0),new(3, 5,0),new(3, 6,0),new(3, 7,0),new(4, 0,0), new(4, 1,0), new(4, 2,0),new(4, 3,0),new(4, 4,0),new(4, 5,0), new(4, 6,0),new(4, 7,0)}),
            new(10, 8, 5, 3, 5, 7, 25, new List<Vector3Int> { new(7, 5,0), new(3, 7,0), new(6, 2,0) }),
            new(5, 5, 2, 4, 6, 9, 12, new List<Vector3Int> { new(1, 3,0), new(4, 4,0), new(2, 2,0) }),
            new(9, 9, 6, 3, 5, 7, 30, new List<Vector3Int> { new(5, 5,0), new(7, 7,0), new(3, 4,0) }),
            new(6, 10, 4, 3, 6, 8, 22, new List<Vector3Int> { new(3, 6,0), new(5, 9,0), new(2, 7,0) }),
            new(10, 10, 5, 2, 4, 7, 35, new List<Vector3Int> { new(8, 8,0), new(4, 5,0), new(6, 3,0) }),
            new(7, 7, 3, 3, 5, 7, 17, new List<Vector3Int> { new(2, 2,0), new(5, 5,0), new(6, 1,0) }),
            new(8, 6, 4, 4, 6, 8, 21, new List<Vector3Int> { new(4, 4,0), new(3, 5,0), new(7, 2,0) }),
            new(9, 5, 3, 3, 5, 7, 20, new List<Vector3Int> { new(6, 3,0), new(4, 2,0), new(2, 4,0) }),
            new(10, 7, 6, 3, 4, 6, 28, new List<Vector3Int> { new(9, 6,0), new(7, 4,0), new(5, 2,0) }),
            new(6, 8, 4, 3, 5, 7, 19, new List<Vector3Int> { new(4, 3,0), new(2, 6,0), new(5, 7,0) }),
            new(8, 9, 5, 3, 5, 7, 26, new List<Vector3Int> { new(6, 5,0), new(3, 7,0), new(7, 8,0) }),
            new(7, 10, 3, 3, 6, 7, 3, new List<Vector3Int> { new(4, 6,0), new(2, 8,0), new(6, 9,0) }),

        };
    }

    public void LoadLevel(int levelIndex)
    {
        levelIndex += 1;
        if (levelIndex >= 0 && levelIndex < levels.Count)
        {
            currentLevel = levels[levelIndex];

            // Apply values to main parameters for easy access
            rows = currentLevel.rows;
            columns = currentLevel.columns;
            colors = currentLevel.colors;
            a = currentLevel.a;
            b = currentLevel.b;
            c = currentLevel.c;
            if (levelIndex == 0)
            {
                isEndlessMode = true;
            }
            else
            {
                isEndlessMode = false;
            }

        }
        else
        {
            Debug.LogError("Invalid level index!");
        }
    }

    public int ShouldBeObstacle(int x, int y, bool initialPopulation)
    {
        if (!initialPopulation)
        {
            return -1;
        }
        if (ParameterData.Instance.currentLevel.objectLocations.Contains(new Vector3Int(x, y, 0)))
        {
            return 0;
        }
        else if (ParameterData.Instance.currentLevel.objectLocations.Contains(new Vector3Int(x, y, 1)))
        {
            return 1;
        }

        return -1;
    }
}
