using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections;
using UnityEngine;


public class MatchableGrid : GridSystem<Blastable>
{
    [SerializeField] private Vector3 offScreenOffset;
    private BlastablePool pool;
    private ScoreManager score;
    private HintIndicator hint;
    [SerializeField] private List<Blastable> possibleMoves;
    [SerializeField] private GameObject spawnHider;
    [SerializeField] private CharacterBarManager characterBarManager;
    private int a;
    private int b;
    private int c;
    private int k;
    private bool isEndlessMode;
    private int moves;
    public event System.Action OnGameOver;
    public event System.Action OnLevelComplete;

    private void Start()
    {
        pool = (BlastablePool)BlastablePool.Instance;
        score = ScoreManager.Instance;
        hint = HintIndicator.Instance;

    }

    /// <summary>
    /// Set the parameters 1st, 2nd, and 3rd icon numbers, number of colors
    /// the offscren offset and the spawnhider
    /// </summary>
    /// <param name="a">The parameter for the first powerup</param>
    /// <param name="b">The parameter for the second powerup</param>
    /// <param name="c">The parameter for the third powerup</param>
    /// <param name="k">The number of colors</param>
    public void SetValues(int a, int b, int c, int k, int maxMoves, bool isEndlessMode = false)
    {
        this.a = a;
        this.b = b;
        this.c = c;
        this.k = k;
        this.isEndlessMode = isEndlessMode;
        this.moves = maxMoves;
        if (!isEndlessMode)
        {
            characterBarManager.UpdateObstacleDisplay(BlastableGridHelper.HasTwoObstacles(), ObstaclesRemaining());
            characterBarManager.UpdateMovesText(maxMoves);
        }
        offScreenOffset = new Vector3(0, Dimensions.y + 1, 0);
        spawnHider.transform.position = new Vector3(0, 0.5f + (Dimensions.y / 2), 0);
    }


    /// <summary>
    /// Populates the grid with random blastables. If initial population is true plays the initial population
    /// animation where blastables drop one by one in a wave like pattern
    /// </summary>
    /// <param name="initialPopulation">Boolean flag to indicate if we are in the initial population</param>
    /// <returns>Returns an IEnumerator we can start as a coroutine and await until the population ends</returns>
    public IEnumerator PopulateGrid(bool initialPopulation = false)
    {
        List<Blastable> newMatchables = new();

        Vector3 onScreenPosition;

        //assign matchables on the grid and add them to the newmatchables list 
        for (int y = 0; y != Dimensions.y; ++y)
            for (int x = 0; x != Dimensions.x; ++x)
                if (IsEmpty(x, y))
                {
                    {
                        int obstacleIndex = ParameterData.Instance.ShouldBeObstacle(x, y, initialPopulation);
                        if (isEndlessMode) { obstacleIndex = -1; }
                        newMatchables.Add(PutBlastableOnGrid(x, y, obstacleIndex));
                    }
                }
        if (initialPopulation)
        {
            EnsureInitialMatch();
            ObstaclesRemaining();
        }

        //move the new matchables to their positions on the grid
        for (int i = 0; i != newMatchables.Count; i++)
        {
            onScreenPosition = transform.position + new Vector3(newMatchables[i].position.x, newMatchables[i].position.y);

            if (i == newMatchables.Count - 1)
            {
                yield return StartCoroutine(newMatchables[i].MoveToPosition(onScreenPosition));
            }
            else
            {
                StartCoroutine(newMatchables[i].MoveToPosition(onScreenPosition));
            }

            //if we are on the initial population wait 0.1 second to give the falling into place one by effect
            if (initialPopulation)
            {
                yield return new WaitForSeconds(0.05f);
            }
        }

    }


    /// <summary>
    /// Ensure there is an initial blastable spot on the grid
    /// </summary>
    private void EnsureInitialMatch()
    {
        int randomPositionX, randomPositionY;

        do
        {

            randomPositionX = UnityEngine.Random.Range(1, Dimensions.x - 1);
            randomPositionY = Random.Range(1, Dimensions.y - 1);
        } while (GetItemAt(randomPositionX, randomPositionY).IsObstacle ||
                 GetItemAt(randomPositionX - 1, randomPositionY).IsObstacle);

        int randomType = Random.Range(0, pool.HowManyTypes);

        pool.ChangeColor(GetItemAt(randomPositionX, randomPositionY), randomType);
        pool.ChangeColor(GetItemAt(randomPositionX - 1, randomPositionY), randomType);
        UpdateSpritesForGroups();
    }


    /// <summary>
    /// Given two integers for the position on the grid to be filled. Fills it with a random blastable.
    /// </summary>
    /// <param name="x">X coordinate of desired spot on the grid</param>
    /// <param name="y">Y coordinate of desired spot on the grid</param>
    /// <returns>Retruns the blastable</returns>
    private Blastable PutBlastableOnGrid(int x, int y, int obstacleIndex)
    {
        Blastable newMatchable;
        if (obstacleIndex != -1)
        {
            newMatchable = pool.GetObstacle(obstacleIndex);
        }
        else
        {
            newMatchable = pool.GetRandomBlastable(k);
        }

        newMatchable.SetSortingOrder(y);

        newMatchable.gameObject.SetActive(true);

        newMatchable.transform.position = transform.position + new Vector3(x, 0) + offScreenOffset;
        newMatchable.position = new Vector2Int(x, y);
        PutItemAt(newMatchable, x, y);
        return newMatchable;
    }

    public IEnumerator CollapseAndFillGrid()
    {
        CollapseGrid();
        yield return StartCoroutine(PopulateGrid());
        if (!isEndlessMode)
        {
            CheckGameState();
        }

    }

    /// <summary>
    /// Collapses any empty spots on the grid  by traversing each column from bottom to the top.
    /// Non idle blastables (the ones already dropping) are skipped.
    /// </summary>
    private void CollapseGrid()
    {
        for (int x = 0; x != Dimensions.x; ++x)
            for (int yEmpty = 0; yEmpty != Dimensions.y - 1; ++yEmpty)
                if (IsEmpty(x, yEmpty))
                    for (int yNotEmpty = yEmpty + 1; yNotEmpty != Dimensions.y; ++yNotEmpty)
                        if (!IsEmpty(x, yNotEmpty) && GetItemAt(x, yNotEmpty).Idle)
                        {
                            //Move blastable from notempty to empty
                            MoveMatchableToPosition(GetItemAt(x, yNotEmpty), x, yEmpty);
                            break;
                        }
    }
    /// <summary>
    /// Moves the given blastable to the given x and y coordinates on the grid.
    /// Should only be called if (x,y) on the grid is empty
    /// </summary>
    /// <param name="toMove">Blastable to be moved</param>
    /// <param name="x">X coordinate of the desired point</param>
    /// <param name="y">Y coordinate of the desired point</param>
    private void MoveMatchableToPosition(Blastable toMove, int x, int y)
    {
        if (!BoundsCheck(toMove.position.x, toMove.position.y))
            Debug.LogError("(" + toMove.position.x + ", " + toMove.position.y + ") is not on the grid.");

        if (!BoundsCheck(x, y))
            Debug.LogError("(" + x + ", " + y + ") is not on the grid.");

        if (!IsEmpty(x, y))
            return;

        Blastable temp = RemoveItemAt(toMove.position.x, toMove.position.y);
        PutItemAt(temp, x, y);
        temp.SetSortingOrder(y);

        toMove.position = new Vector2Int(x, y);

        StartCoroutine(toMove.MoveToPosition(transform.position + new Vector3(x, y)));
    }

    /// <summary>
    /// Given a blastable checks if the blastable can be blasted. Called when the user taps on a blastable
    /// </summary>
    /// <param name="toBeBlasted">Blastable to be checked for a valid blast</param>
    /// <returns>Returns and IEnumerator we can yield return if we want to await for the blast to resolve</returns>
    public IEnumerator TryBlast(Blastable toBeBlasted)
    {
        Blastable copy;
        copy = toBeBlasted;

        hint.CancelHint();
        Blast blasts = GetBlast(copy);

        if (blasts != null)
        {
            yield return StartCoroutine(score.ResolveBlast(blasts, a, b, c));
            yield return StartCoroutine(CollapseAndFillGrid());
            CheckPossibleMoves();
            UpdateSpritesForGroups();
        }
    }

    private Blast GetBlast(Blastable toBlast)
    {
        Blast blast = new(toBlast);

        blast = GetBlastedArea(toBlast);


        if (blast.Count == 1 && !blast.Blastables[0].IsPowerUp)
        {
            return null;
        }

        return blast;
    }

    private Blast GetBlastedArea(Blastable startMatchable)
    {
        Blast blast = new();
        HashSet<Vector2Int> visited = new();
        Queue<Blastable> toCheck = new();

        toCheck.Enqueue(startMatchable);
        visited.Add(startMatchable.position);
        blast.AddBlastable(startMatchable);

        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        while (toCheck.Count > 0)
        {
            Blastable current = toCheck.Dequeue();

            foreach (Vector2Int direction in directions)
            {
                Vector2Int neighborPosition = current.position + direction;

                if (BoundsCheck(neighborPosition) && !IsEmpty(neighborPosition) && !visited.Contains(neighborPosition))
                {
                    Blastable neighbor = GetItemAt(neighborPosition);

                    if (neighbor.Type == startMatchable.Type && neighbor.Idle && neighbor.Type != -1 && neighbor.Type != -2)
                    {
                        toCheck.Enqueue(neighbor);
                        visited.Add(neighborPosition);
                        blast.AddBlastable(neighbor);
                    }
                }
            }
        }

        return blast;
    }
    private int ScanForMoves()
    {
        possibleMoves = new List<Blastable>();

        for (int y = 0; y != Dimensions.y; ++y)
            for (int x = 0; x != Dimensions.x; ++x)
                if (BoundsCheck(x, y) && !IsEmpty(x, y) && CanBlast(GetItemAt(x, y)))
                {
                    possibleMoves.Add(GetItemAt(x, y));
                }

        return possibleMoves.Count;
    }

    public void CheckPossibleMoves()
    {
        if (ScanForMoves() == 0)
        {
            hint.CancelHint();
            StartCoroutine(ShuffleGrid());
        }
        else
        {
            hint.StartAutoHint(possibleMoves[Random.Range(0, possibleMoves.Count)].transform);
        }
    }

    private IEnumerator ShuffleGrid()
    {
        while (!WholeGridIdle())
        {
            yield return new WaitForSeconds(0.1f);
        }

        StartCoroutine(Shuffle());

        EnsureInitialMatch();
    }
    private bool WholeGridIdle()
    {
        for (int y = 0; y < Dimensions.y; y++)
        {
            for (int x = 0; x < Dimensions.x; x++)
            {
                if (!GetItemAt(x, y).Idle)
                {
                    return false;
                }
            }
        }
        return true;
    }
    public IEnumerator Shuffle()
    {
        while (!WholeGridIdle())
        {
            yield return new WaitForSeconds(0.1f);
        }

        List<Vector2Int> firstHalfPositions = new();
        List<Vector2Int> secondHalfPositions = new();

        // Divide the grid into first and second halves
        for (int y = 0; y < Dimensions.y; y++)
        {
            for (int x = 0; x < Dimensions.x; x++)
            {
                Vector2Int position = new(x, y);
                if (y < Dimensions.y / 2)
                {
                    firstHalfPositions.Add(position);
                }
                else
                {
                    secondHalfPositions.Add(position);
                }
            }
        }
        secondHalfPositions = secondHalfPositions.OrderBy(pos => Random.value).ToList();

        List<Coroutine> swapCoroutines = new();

        for (int i = 0; i < firstHalfPositions.Count && i < secondHalfPositions.Count; i++)
        {
            Vector2Int firstHalfPos = firstHalfPositions[i];
            Vector2Int secondHalfPos = secondHalfPositions[i];

            Blastable firstBlastable = GetItemAt(firstHalfPos.x, firstHalfPos.y);
            Blastable secondBlastable = GetItemAt(secondHalfPos.x, secondHalfPos.y);

            if (firstBlastable == null || secondBlastable == null)
                continue;

            Blastable[] toBeSwapped = new Blastable[] { firstBlastable, secondBlastable };

            Coroutine swapCoroutine = StartCoroutine(Swap(toBeSwapped));
            swapCoroutines.Add(swapCoroutine);
        }

        foreach (Coroutine swap in swapCoroutines)
        {
            yield return swap;
        }
        UpdateSpritesForGroups();
    }
    private IEnumerator Swap(Blastable[] toBeSwapped)
    {
        //  swap them in the grid data structure
        SwapItemsAt(toBeSwapped[0].position, toBeSwapped[1].position);

        //  tell the blastables their new positions
        (toBeSwapped[1].position, toBeSwapped[0].position) = (toBeSwapped[0].position, toBeSwapped[1].position);

        //  get the world positions of both
        Vector3[] worldPosition = new Vector3[2];
        worldPosition[0] = toBeSwapped[0].transform.position;
        worldPosition[1] = toBeSwapped[1].transform.position;
        //TODO bunu da diğeri gibi düzgüng get yap
        int tempSortingOrder = toBeSwapped[0].GetSortingOrder();
        toBeSwapped[0].SetSortingOrder(toBeSwapped[1].GetSortingOrder());
        toBeSwapped[1].SetSortingOrder(tempSortingOrder);

        //  move them to their new positions on screen
        StartCoroutine(toBeSwapped[0].MoveToPosition(worldPosition[1], false));
        yield return StartCoroutine(toBeSwapped[1].MoveToPosition(worldPosition[0], false));
    }



    private bool CanBlast(Blastable blastable)
    {
        return BlastableGridHelper.CanBlast(blastable, BoundsCheck, IsEmpty, GetItemAt);
    }


    public void UpdateSpritesForGroups()
    {
        HashSet<Vector2Int> processedPositions = new();

        for (int y = 0; y < Dimensions.y; y++)
        {
            for (int x = 0; x < Dimensions.x; x++)
            {
                Vector2Int position = new(x, y);
                if (!processedPositions.Add(position) || IsEmpty(position))
                    continue;

                Blast group = GetBlastedArea(GetItemAt(position));
                foreach (Blastable blastable in group.Blastables)
                    processedPositions.Add(blastable.position);

                int iconLevel = (group.Count > c) ? 3 :
                                (group.Count > b) ? 2 :
                                (group.Count > a) ? 1 : 0;

                foreach (Blastable blastable in group.Blastables)
                    pool.UpdateIcon(blastable, iconLevel);
            }
        }
    }

    public IEnumerator MatchAllAdjacent(Blastable powerup)
    {
        Blast allAdjacent = new();

        for (int y = powerup.position.y - 1; y != powerup.position.y + 2; ++y)
            for (int x = powerup.position.x - 1; x != powerup.position.x + 2; ++x)
                if (BoundsCheck(x, y) && !IsEmpty(x, y) && GetItemAt(x, y).Idle)
                    allAdjacent.AddBlastable(GetItemAt(x, y));

        yield return StartCoroutine(score.ResolveBlast(allAdjacent, a, b, c, true));
    }

    /// <summary>
    /// Given the gem powerup, selects a random adjacent blastable and blasts everything with the same type.
    /// </summary>
    /// <param name="gem"></param>
    /// <returns></returns>
    public IEnumerator MatchEverythingOfRandomType(Blastable gem)
    {
        // Get a random existing blastable from the grid
        int targetType = GetAdjacentBlastableType(gem.position);
        Blast everythingByType = new();

        for (int y = 0; y < Dimensions.y; ++y)
        {
            for (int x = 0; x < Dimensions.x; ++x)
            {
                if (BoundsCheck(x, y) && !IsEmpty(x, y))
                {
                    Blastable item = GetItemAt(x, y);
                    if (item != null && item.Idle && item.Type == targetType)
                    {
                        everythingByType.AddBlastable(item);
                    }
                }
            }
        }
        yield return StartCoroutine(score.ResolveBlast(everythingByType, a, b, c, true));
    }
    private int GetAdjacentBlastableType(Vector2Int position)
    {
        return BlastableGridHelper.GetAdjacentBlastableType(position, BoundsCheck, IsEmpty, GetItemAt);
    }

    /// <summary>
    /// Given the powerup desired to be blasted resolves everything in the same row and the same column as the powerup.
    /// </summary>
    /// <param name="powerup">Powerup to be blasted</param>
    /// <returns>Returns and IEnumerator we can yield return for 
    /// if we want to wait for the whole blast animaton to terminate.</returns>
    public IEnumerator ResolveWaveBlast(Blastable powerup)
    {
        int maxDistance = Mathf.Max(Dimensions.x, Dimensions.y);

        for (int distance = 0; distance < maxDistance; distance++)
        {
            Blast currentWave = new();
            bool foundMatch = false;

            int x = powerup.position.x;
            int y = powerup.position.y;

            if (BoundsCheck(x, y + distance) && !IsEmpty(x, y + distance) && GetItemAt(x, y + distance).Idle)
            {
                currentWave.AddBlastable(GetItemAt(x, y + distance));
                foundMatch = true;
            }
            if (BoundsCheck(x, y - distance) && !IsEmpty(x, y - distance) && GetItemAt(x, y - distance).Idle)
            {
                currentWave.AddBlastable(GetItemAt(x, y - distance));
                foundMatch = true;
            }
            if (BoundsCheck(x + distance, y) && !IsEmpty(x + distance, y) && GetItemAt(x + distance, y).Idle)
            {
                currentWave.AddBlastable(GetItemAt(x + distance, y));
                foundMatch = true;
            }
            if (BoundsCheck(x - distance, y) && !IsEmpty(x - distance, y) && GetItemAt(x - distance, y).Idle)
            {
                currentWave.AddBlastable(GetItemAt(x - distance, y));
                foundMatch = true;
            }

            if (foundMatch)
            {
                yield return StartCoroutine(score.ResolveBlast(currentWave, a, b, c, true));
                yield return new WaitForSeconds(0.05f / distance);
            }
        }
    }
    public int[] ObstaclesRemaining()
    {
        if (isEndlessMode)
        {
            return null;
        }
        int[] obstaclesRemaining = BlastableGridHelper.GetObstaclesRemaining(isEndlessMode, Dimensions, BoundsCheck, IsEmpty, GetItemAt);
        characterBarManager.UpdateObstacleDisplay(BlastableGridHelper.HasTwoObstacles(), obstaclesRemaining);
        return obstaclesRemaining;
    }
    public void CheckGameState()
    {
        if (IsLevelComplete())
        {
            characterBarManager.SetCharacterBarActive(false);
            OnLevelComplete?.Invoke();
        }
        else if (IsGameOver())
        {
            characterBarManager.SetCharacterBarActive(false);
            OnGameOver?.Invoke();
        }
    }
    private bool IsGameOver()
    {
        moves -= 1;
        characterBarManager.UpdateMovesText(moves);

        if (moves <= 0)
        {
            return true;
        }
        return false;
    }
    private bool IsLevelComplete()
    {
        int[] obstaclesRemaining = ObstaclesRemaining();
        if (obstaclesRemaining[0] + obstaclesRemaining[1] == 0)
        {
            return true;
        }
        return false;
    }
}
