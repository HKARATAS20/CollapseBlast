using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BlastablePool : ObjectPool<Blastable>
{

    public int howManyTypes;
    public int HowManyTypes
    {
        get { return howManyTypes; }
        set { howManyTypes = value; }
    }
    [SerializeField] private Sprite[] blueSprites;
    [SerializeField] private Sprite[] greenSprites;
    [SerializeField] private Sprite[] pinkSprites;
    [SerializeField] private Sprite[] purpleSprites;
    [SerializeField] private Sprite[] redSprites;
    [SerializeField] private Sprite[] yellowSprites;
    [SerializeField] private Sprite[] particleSprites;
    [SerializeField] private Sprite[] runeStoneSprites;
    [SerializeField] private Sprite[] chestSprites;

    private Sprite[][] obstacleSprites;

    private readonly int[] healths = new int[] { 2, 1 };

    private Sprite[][] sprites;

    [SerializeField] private Sprite[] powerUps;

    protected override void Init()
    {
        sprites = new Sprite[howManyTypes][];
        obstacleSprites = new Sprite[2][];

        sprites[0] = blueSprites;
        sprites[1] = greenSprites;
        sprites[2] = pinkSprites;
        sprites[3] = purpleSprites;
        sprites[4] = redSprites;
        sprites[5] = yellowSprites;
        obstacleSprites[0] = runeStoneSprites;
        obstacleSprites[1] = chestSprites;
    }

    public void RandomizeType(Blastable toRandomize, int k)
    {
        int random = Random.Range(0, howManyTypes);

        toRandomize.SetType(random, sprites[random][0], particleSprites[random]);
    }
    public Blastable GetRandomBlastable(int k)
    {
        Blastable randomMatchable = GetPooledObject();
        RandomizeType(randomMatchable, k);
        return randomMatchable;
    }

    public Blastable GetObstacle(int obstacleIndex)
    {

        Blastable obstacle = GetPooledObject();
        obstacle.MakeObstacle(obstacleSprites[obstacleIndex], healths[obstacleIndex], obstacleIndex);
        return obstacle;

    }

    public int NextType(Blastable blastable)
    {
        int nextType = (blastable.Type + 1) % howManyTypes;

        blastable.SetType(nextType, sprites[nextType][0], particleSprites[nextType]);

        return nextType;

    }

    public void ChangeColor(Blastable toChange, int type)
    {
        toChange.SetType(type, sprites[type][0], particleSprites[type]);
    }

    public void UpdateIcon(Blastable blastable, int iconIndex)
    {
        if (blastable.Type == -1 || blastable.Type == -2)
        {
            return;
        }
        blastable.SetType(blastable.Type, sprites[blastable.Type][iconIndex], particleSprites[blastable.Type]);
    }
    public void SetPowerUp(Blastable blastable, int powerUp, int a, int b, int c)
    {
        powerUp += 1;
        if (powerUp > c)
        {
            blastable.Upgrade(BlastType.cIcon, powerUps[2]);
            blastable.SetType(-1, powerUps[2], particleSprites[6]);
        }
        else if (powerUp > b)
        {
            blastable.Upgrade(BlastType.bIcon, powerUps[1]);
            blastable.SetType(-1, powerUps[1], particleSprites[6]);
        }
        else
        {
            blastable.Upgrade(BlastType.aIcon, powerUps[0]);
            blastable.SetType(-1, powerUps[0], particleSprites[6]);
        }
    }

    public override void ReturnObjectToPool(Blastable toBeReturned)
    {
        toBeReturned.powerup = BlastType.invalid;
        toBeReturned.obstacleType = ObstacleType.not;

        base.ReturnObjectToPool(toBeReturned);
    }

}
