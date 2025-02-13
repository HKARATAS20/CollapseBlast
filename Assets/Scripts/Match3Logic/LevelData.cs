using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData
{
    public int rows, columns, colors;
    public int a, b, c;
    public int maxMoves;
    public List<Vector3Int> objectLocations;

    public LevelData(int rows, int columns, int colors, int a, int b, int c, int maxMoves, List<Vector3Int> objectLocations)
    {
        this.rows = rows;
        this.columns = columns;
        this.colors = colors;
        this.a = a;
        this.b = b;
        this.c = c;
        this.maxMoves = maxMoves;
        this.objectLocations = objectLocations;
    }
}
