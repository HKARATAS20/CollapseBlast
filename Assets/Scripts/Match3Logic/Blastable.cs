using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObstacleType
{
    runestone,
    chest,
    not
}
[RequireComponent(typeof(SpriteRenderer))]
public class Blastable : Movable
{

    private int type;

    public int Type
    {
        get { return type; }
    }

    public BlastType powerup = BlastType.invalid;

    public ObstacleType obstacleType;

    public bool IsPowerUp
    {
        get
        {
            return powerup != BlastType.invalid;
        }
    }

    public bool IsObstacle
    {
        get
        {
            return obstacleType != ObstacleType.not;
        }
    }

    private BlastablePool pool;
    private MatchableGrid grid;
    private Cursor cursor;

    private int orderInLayer;

    private SpriteRenderer spriteRenderer;

    private Sprite particleSrite;

    public Vector2Int position; //where blastable is on the grid

    [SerializeField] private ParticleSystem particles;

    public int health;

    public Sprite[] obstacleStates;


    private void Awake()
    {
        ParticlesSetup();

        spriteRenderer = GetComponent<SpriteRenderer>();

        cursor = Cursor.Instance;
        pool = (BlastablePool)BlastablePool.Instance;
        grid = (MatchableGrid)MatchableGrid.Instance;
        obstacleType = ObstacleType.not;
    }

    public void ParticlesSetup()
    {
        if (particles != null)
        {
            // Instantiate the particle system at the current position
            particles = Instantiate(particles, Vector3.zero, Quaternion.identity);
            GameObject particlesObject = GameObject.Find("Particles");
            particles.transform.parent = particlesObject.transform;
            particles.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Particle system prefab is not assigned!");
        }
    }

    public void SetType(int type, Sprite sprite, Sprite particleSprite)
    {
        this.type = type;
        spriteRenderer.sprite = sprite;
        particleSrite = particleSprite;
    }
    public IEnumerator Resolve(Transform collectionPoint, bool isPowerUp = false)
    {
        bool playParticles = true;

        if (IsObstacle)
        {
            DamageObstacle();
            yield break;
        }
        else
        {
            //if match results in a powerup
            if (isPowerUp)
            {
                playParticles = false;
                SetSortingOrder(13);
                yield return StartCoroutine(MoveToTransform(collectionPoint));
                SetSortingOrder(1);
            }

            //if the blasted blastable is a powerup
            if (powerup != BlastType.invalid)
            {
                // resolve a match4 powerup
                if (powerup == BlastType.aIcon)
                {
                    // score everything adjacent to this
                    yield return grid.ResolveWaveBlast(this);
                }
                if (powerup == BlastType.bIcon)
                {
                    // score everything adjacent to this
                    yield return grid.MatchAllAdjacent(this);
                }
                // resolve a cross powerup
                if (powerup == BlastType.cIcon)
                {
                    yield return grid.MatchEverythingOfRandomType(this);
                }
                powerup = BlastType.invalid;
            }

            if (playParticles)
            {
                PlayParticleEffects();
            }

            pool.ReturnObjectToPool(this);

            yield return null;
        }
    }
    private void OnMouseDown()
    {
        if (IsObstacle)
        {
            return;
        }
        cursor.ItemTapped(this);
        cursor.SelectFirst(this);
    }

    private void OnMouseUp()
    {
        cursor.SelectFirst(null);
    }

    public override string ToString()
    {
        return gameObject.name;
    }

    public void SetSortingOrder(int order)
    {
        orderInLayer = order;
        spriteRenderer.sortingOrder = order;

    }
    public int GetSortingOrder()
    {
        return orderInLayer;

    }
    private void PlayParticleEffects()
    {
        // Move the particle system to the position of the current blastable
        particles.transform.position = gameObject.transform.position;

        // Get the ParticleSystem's MainModule to modify its properties
        //var mainModule = particles.main;

        // Get the TextureSheetAnimationModule to assign a sprite
        var textureSheetAnimation = particles.textureSheetAnimation;

        // Clear existing sprites (optional, to avoid conflicts)
        textureSheetAnimation.RemoveSprite(0);

        // Assign the new sprite
        textureSheetAnimation.AddSprite(particleSrite);

        // Enable and play the particle system
        particles.gameObject.SetActive(true);
        particles.Play();
    }

    public Blastable Upgrade(BlastType powerupType, Sprite powerupSprite)
    {
        this.SetType(-1, powerupSprite, particleSrite);
        powerup = powerupType;
        return this;
    }

    public void MakeObstacle(Sprite[] obstacleSprites, int health, int obstacleIndex)
    {
        this.health = health;
        obstacleType = obstacleIndex == 0 ? ObstacleType.runestone : ObstacleType.chest;
        this.obstacleStates = obstacleSprites;
        this.SetType(-2, obstacleSprites[0], obstacleSprites[0]);
    }

    public bool DamageObstacle()
    {
        this.health -= 1;
        if (this.health <= 0)
        {
            PlayParticleEffects();

            pool.ReturnObjectToPool(this);

            return true;
        }
        else
        {
            this.SetType(-2, obstacleStates[health], particleSrite);
            return false;
        }
    }

}
