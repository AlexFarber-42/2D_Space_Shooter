using System.Collections;
using UnityEngine;
using UnityEngine.WSA;

public class Turret : MonoBehaviour
{
    [Header("Turret Settings")]
    [SerializeField] private GameObject projectile;
    [SerializeField] private float fireRate = 1.5f;
    [SerializeField] private bool targetPlayer = true;

    [SerializeField]
    [Tooltip("Positional modifier for where the bullet should spawn in relation to the turret itself")] 
    private float spawnOffset;

    [SerializeField] private Turret_Dir[] turretDirections;
    [SerializeField] private Turret_Dir.Direction startingDir = Turret_Dir.Direction.Up;
    
    [SerializeField]
    [Tooltip("The threshold for detection of entering a new rotation form, a larger value means a larger area for the user to be considered directly 'under' or 'over'")] 
    private float rotThreshold = .75f;

    private Transform playerTrack;
    private Turret_Dir.Direction dir;
    private Vector3 offset;

    private int projectilesFired = 0;

    private void Start()
    {
        playerTrack = Player.Instance.transform;
        dir         = startingDir;

        SetTurretAngle();
        StartCoroutine(Fire());
    }
    
    private IEnumerator Fire()
    {
        while (true)
        {
            yield return new WaitForSeconds(fireRate);

            GameObject projInstance = Pools.Instance.SpawnObject(Pools.PoolType.Projectile, projectile, transform.position + offset, projectile.transform.rotation);
            Projectile proj = projInstance.GetComponent<Projectile>();

            proj.IsHostileProjectile = true;
            proj.FireProjectile(GatherRotation(), GatherDirection());
            projectilesFired++;
            // TODO --> To be used as a fun data point for the user to check total projectiles that were fired at them at the end of a round
        }
    }
    
    private void FixedUpdate()
    {
        // On my life trying to use quaternions is a hassle
        // So instead we are determining the general direction the player is in relation to the turret
        Vector2 shift = playerTrack.position - transform.position;
        float xVal = shift.x;
        float yVal = shift.y;

        bool aboveXThresh = xVal <= 0f + rotThreshold;
        bool belowXThresh = xVal >= 0f - rotThreshold;
        bool aboveYThresh = yVal <= 0f + rotThreshold;
        bool belowYThresh = yVal >= 0f - rotThreshold;

        // iIf x is roughly 0, then we are either directly above or below the turret
        if (aboveXThresh && belowXThresh)
        {
            if (yVal < 0f)
                dir = Turret_Dir.Direction.Down;
            else if (yVal > 0f)
                dir = Turret_Dir.Direction.Up;
        }

        // If y is roughly 0, then we are either directly left or right of the turret
        if (aboveYThresh && belowYThresh)
        {
            if (xVal < 0f)
                dir = Turret_Dir.Direction.Left;
            else if (xVal > 0f)
                dir = Turret_Dir.Direction.Right;
        }

        // Otherwise we are in a diagonal direction based on the negative/positive value of the turret
        if (!aboveXThresh) // This indicates bottom right or top right
        {
            if (!aboveYThresh) // This indicates top right
                dir = Turret_Dir.Direction.UpRight;
            else if (!belowYThresh) // This indicates bottom right
                dir = Turret_Dir.Direction.DownRight;
        }
        else if (!belowXThresh) // This indicates bottom left or top left
        {
            if (!aboveYThresh) // This indicates top left
                dir = Turret_Dir.Direction.UpLeft;
            else if (!belowYThresh) // This indicates bottom left
                dir = Turret_Dir.Direction.DownLeft;
        }

        // Debug.Log($"Direction: {dir}\nShift Values: X - {xVal} Y - {yVal}");

        SetTurretAngle();
    }

    private void SetTurretAngle()
    {
        offset = GatherDirection() * spawnOffset;

        SetSprite();
    }

    private Vector2 GatherDirection()
    {
        return dir switch
        {
            Turret_Dir.Direction.Up         => Vector2.up,
            Turret_Dir.Direction.UpRight    => (Vector2.up + Vector2.right).normalized,
            Turret_Dir.Direction.Right      => Vector2.right,
            Turret_Dir.Direction.DownRight  => (Vector2.down + Vector2.right).normalized,
            Turret_Dir.Direction.Down       => Vector2.down,
            Turret_Dir.Direction.DownLeft   => (Vector2.down + Vector2.left).normalized,
            Turret_Dir.Direction.Left       => Vector2.left,
            Turret_Dir.Direction.UpLeft     => (Vector2.up + Vector2.left).normalized,
            _                               => Vector2.zero,
        };
    }

    private Vector3 GatherRotation()
    {
        return dir switch
        {
            Turret_Dir.Direction.Up         => new Vector3(0, 0, 180),
            Turret_Dir.Direction.UpRight    => new Vector3(0, 0, 135),
            Turret_Dir.Direction.Right      => new Vector3(0, 0, 90),
            Turret_Dir.Direction.DownRight  => new Vector3(0, 0, 45),
            Turret_Dir.Direction.Down       => new Vector3(0, 0, 0),
            Turret_Dir.Direction.DownLeft   => new Vector3(0, 0, 315),
            Turret_Dir.Direction.Left       => new Vector3(0, 0, 270),
            Turret_Dir.Direction.UpLeft     => new Vector3(0, 0, 235),
            _                               => new Vector3(0, 0, 0)
        };
    }

    // Instead of setting up an animator, just detect the change the sprites based on the calculated direction
    private void SetSprite()
    {
        foreach (Turret_Dir td in turretDirections)
        {
            if (td.direction == dir)
            {
                GetComponent<SpriteRenderer>().sprite = td.sprite;
                // Debug.Log($"Changed Sprite {dir}");
                return;
            }
        }
    }
}

[System.Serializable]
public struct Turret_Dir
{
    public enum Direction
    {
        Up,
        UpRight,
        Right,
        DownRight,
        Down,
        DownLeft,
        Left,
        UpLeft,
    }

    public Direction direction;
    public Sprite sprite;
}
