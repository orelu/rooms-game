using UnityEngine;
using System.Collections;

public class RockLauncherLogic : MonoBehaviour
{
    private bool isRunning = false;

    public GameObject projectilePrefab;  // Projectile to spawn
    public Transform firePoint;          // Spawn point for projectile
    public float projectileForce = -25f;

    public EnemyBaseBehavior enemyStats;
    private Transform playerTransform;   // Will be assigned at runtime

    public float detectionRange = 10f;

    void Start()
    {
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("Player GameObject not found! Make sure it's named 'Player'.");
        }
    }

    void Update()
    {
        if (!isRunning && enemyStats.health > 0 && PlayerInRange())
        {
            StartCoroutine(RunFunctionRepeatedly());
        }
    }

    bool PlayerInRange()
    {
        if (playerTransform == null) return false;

        float distance = Vector2.Distance(transform.position, playerTransform.position);
        return distance < detectionRange;
    }

    IEnumerator RunFunctionRepeatedly()
    {
        isRunning = true;
        while (enemyStats.health > 0)
        {
            if (!PlayerInRange())
            {
                isRunning = false;
                yield break;
            }

            yield return StartCoroutine(AttackRoutine());
            yield return new WaitForSeconds(10f);
        }
        isRunning = false;
    }

    IEnumerator AttackRoutine()
    {
        int bulletsToShoot = 5;
        float intervalBetweenShots = 0.25f;

        for (int i = 0; i < bulletsToShoot; i++)
        {
            ShootProjectile();
            yield return new WaitForSeconds(intervalBetweenShots);
        }
    }

    void ShootProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = firePoint.right * projectileForce;
        }
    }
}
