
using System;
using System.Collections.Generic;
using UnityEngine;

public static class BlastableGridHelper
{
    public static bool CanBlast(Blastable blastable, Func<Vector2Int, bool> boundsCheck, Func<Vector2Int, bool> isEmpty, Func<Vector2Int, Blastable> getItemAt)
    {
        if (blastable.IsPowerUp)
            return true;
        if (blastable.IsObstacle)
            return false;

        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (Vector2Int direction in directions)
        {
            Vector2Int neighborPosition = blastable.position + direction;
            if (boundsCheck(neighborPosition) && !isEmpty(neighborPosition))
            {
                Blastable neighbor = getItemAt(neighborPosition);
                if (neighbor.Type == blastable.Type && neighbor.Idle)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public static int GetAdjacentBlastableType(Vector2Int position, Func<int, int, bool> boundsCheck, Func<int, int, bool> isEmpty, Func<int, int, Blastable> getItemAt)
    {
        Vector2Int[] directions = { new(0, 1), new(0, -1), new(1, 0), new(-1, 0) };
        int startIndex = UnityEngine.Random.Range(0, directions.Length);

        for (int i = 0; i < directions.Length; i++)
        {
            Vector2Int dir = directions[(startIndex + i) % directions.Length];
            int newX = position.x + dir.x;
            int newY = position.y + dir.y;

            if (boundsCheck(newX, newY) && !isEmpty(newX, newY))
            {
                Blastable neighbor = getItemAt(newX, newY);
                if (neighbor != null && neighbor.Idle && neighbor.Type != -1 && neighbor.Type != -2)
                    return neighbor.Type;
            }
        }
        return 0;
    }

    public static bool HasTwoObstacles()
    {
        List<Vector3Int> objects = ParameterData.Instance.currentLevel.objectLocations;
        HashSet<int> uniqueZValues = new();

        foreach (var obj in objects)
        {
            uniqueZValues.Add(obj.z);
            if (uniqueZValues.Count > 1)
                return true;
        }
        return false;
    }

    public static int[] GetObstaclesRemaining(bool isEndlessMode, Vector2Int dimensions, Func<int, int, bool> boundsCheck, Func<int, int, bool> isEmpty, Func<int, int, Blastable> getItemAt)
    {
        int[] obstaclesRemaining = { 0, 0 };

        if (isEndlessMode)
            return obstaclesRemaining;

        for (int y = 0; y < dimensions.y; ++y)
        {
            for (int x = 0; x < dimensions.x; ++x)
            {
                if (boundsCheck(x, y) && !isEmpty(x, y))
                {
                    Blastable item = getItemAt(x, y);
                    if (item != null && item.Type == -2)
                    {
                        if (item.obstacleType == ObstacleType.runestone)
                        {
                            obstaclesRemaining[0] += 1;
                        }
                        else if (item.obstacleType == ObstacleType.chest)
                        {
                            obstaclesRemaining[1] += 1;
                        }
                    }
                }
            }
        }
        return obstaclesRemaining;
    }
}
