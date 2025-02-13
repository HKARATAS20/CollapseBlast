using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ScoreManager : Singleton<ScoreManager>
{
    private MatchableGrid grid;
    private BlastablePool pool;

    [SerializeField]
    private Transform collectionPoint;
    private Text scoreText;

    private int score;

    public int Score { get { return score; } }

    protected override void Init()
    {
        scoreText = GetComponent<Text>();
    }
    private void Start()
    {
        grid = (MatchableGrid)MatchableGrid.Instance;
        pool = (BlastablePool)BlastablePool.Instance;
    }

    public void AddScore(int amount)
    {
        score += amount;
        scoreText.text = "" + score;
    }

    /// <summary>
    /// This function is called when there is a valid blast to be resolved and resolves it and increments the score.
    /// </summary>
    /// <param name="toResolve">The blast we want to resolve</param>
    /// <param name="a">The parameter for the first powerup</param>
    /// <param name="b">The parameter for the second powerup</param>
    /// <param name="c">The parameter for the third powerup</param>
    /// <param name="fromPowerup">A boolean flag telling us if this blast we are resolving originated from a powerup
    /// exploding, defaults to false. </param>
    /// <returns>Returns an IEnumerator that we can use to wait until the blast finishes resolving. </returns>
    internal IEnumerator ResolveBlast(Blast toResolve, int a, int b, int c, bool fromPowerup = false)
    {
        if (toResolve.Count <= 0) yield break;

        List<Coroutine> runningCoroutines = new List<Coroutine>();

        Blastable powerup = toResolve.Blastables[0];
        bool isPowerUp = false;

        if (toResolve.Count > a && !fromPowerup)
        {
            isPowerUp = true;
            powerup.SetSortingOrder(30);
            toResolve.Blastables.RemoveAt(0);
            collectionPoint = powerup.transform;
            pool.SetPowerUp(powerup, toResolve.Count, a, b, c);
        }

        for (int i = 0; i < toResolve.Count; i++)
        {
            Blastable blastable = toResolve.Blastables[i];

            if (!(blastable.IsObstacle && blastable.health > 1))
            {
                grid.RemoveItemAt(blastable.position);
            }
            if (i == toResolve.Count - 1)
            {
                yield return StartCoroutine(blastable.Resolve(collectionPoint, isPowerUp));
            }
            else
            {
                runningCoroutines.Add(StartCoroutine(blastable.Resolve(collectionPoint, isPowerUp)));
            }
        }

        // Wait for all coroutines to complete
        foreach (var coroutine in runningCoroutines)
        {
            yield return coroutine;
        }

        AddScore(toResolve.Count * toResolve.Count);

        yield return null;
    }

}
