using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemLogic : MonoBehaviour
{
    [HideInInspector] public GameObject interactingObject;

    public static ItemLogic instance;

    [HideInInspector] public bool giveAttackBonus;

    [HideInInspector] public float attackCooldown;
    [HideInInspector] public float maxAttackCooldown = 1;

    [HideInInspector] public GameObject player;

    [HideInInspector] public Animator playerAnimator;

    [HideInInspector] public Transform attackPoint;

    [HideInInspector] public LayerMask attackables;

    float nextAttackTime = 0f;

    // Ghost trail parameters
    public float ghostSpawnInterval = 0.02f;  // seconds between ghost spawns
    public float ghostLifetime = 0.2f;        // how long each ghost fades out
    public int ghostCountPerSwing = 10;       // how many ghosts to spawn per swing

    private Transform itemHolder; 
    private ParticleSystem slashParticles;


    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of ItemLogic found.");
            return;
        }
        instance = this;

        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
            if (player == null)
            {
                Debug.LogError("Player not found in scene!");
                return;
            }
        }

        // Find playerAnimator if not assigned
        if (playerAnimator == null)
        {
            playerAnimator = player.GetComponent<Animator>();
        }

        // Find AttackPoint as child of player if not assigned
        if (attackPoint == null)
        {
            attackPoint = player.transform.Find("AttackPoint");
            if (attackPoint == null)
            {
                Debug.LogWarning("AttackPoint child not found under Player.");
            }
        }

        // Set attackables to "Attackables" layer if default
        if (attackables == 0)
        {
            attackables = LayerMask.GetMask("Attackables");
        }

        // Try to find sword's SpriteRenderer - assuming the sword is a child of the player or attackPoint
        // Adjust this line if your sword is elsewhere
        if (itemHolder == null)
        {
            itemHolder = player.transform.Find("ItemHolder");
            if (itemHolder == null)
            {
                Debug.LogWarning("ItemHolder child not found under Player.");
            } else {
                Transform particleChild = itemHolder.Find("SlashParticles");
                if (particleChild != null)
                {
                    slashParticles = particleChild.GetComponent<ParticleSystem>();
                }
                else
                {
                    Debug.LogWarning("SlashParticles child not found under ItemHolder.");
                }
            }
            
        }

        
    }

    public virtual bool Use(int useID, Item i)
    {
        if (player == null)
        {
            Debug.LogError("Player not set or found.");
            return false;
        }

        PlayerStats stats = PlayerStats.instance;
        bool isGrounded = player.GetComponent<PlayerController>().m_Grounded;
        int itemID = i != null ? i.itemID : -1;

        bool broken = true;

        if (interactingObject == null)
        {
            if (i != null)
            {
                if (useID == 0)
                {
                    swingAttack(playerAnimator, i);
                    broken = false;
                }
                else
                {
                    switch (itemID)
                    {
                        case 2: stats.money += 5; break;
                        case 5: stats.health += 10; break;
                        case 6: stats.energy += 10; break;
                        case 7: stats.maxEnergy += 10; break;
                        case 8: stats.maxHealth += 10; break;
                        case 9: stats.maxEnergy += 10; break;
                        case 11:
                        case 12:
                        case 13:
                            if (isGrounded)
                            {
                                float jumpForce = itemID == 11 ? 10 : itemID == 12 ? 15 : 20;
                                player.GetComponent<Rigidbody2D>().velocity += new Vector2(0, jumpForce);
                            }
                            else
                            {
                                broken = false;
                            }
                            break;
                        default:
                            broken = false;
                            break;
                    }
                }

                return broken;
            }
        }
        else if (interactingObject.GetComponent<IronDoorLogic>() != null && i != null && i.itemID == 14)
        {
            interactingObject.GetComponent<IronDoorLogic>().ClickLogic();
            return true;
        }
        else if (interactingObject.GetComponent<VaultDoorLogic>() != null && i != null && i.itemID == 15)
        {
            interactingObject.GetComponent<VaultDoorLogic>().ClickLogic();
            return true;
        }

        return false;
    }

    public void loopAttack(Animator playerAnimator, Item i)
    {
        if (attackCooldown < 0.5f)
        {
            float damage = PlayerStats.instance.damage.getValue();
            if (attackCooldown > 0)
            {
                damage *= (1 - attackCooldown) / 1.5f;
            }
            if (giveAttackBonus)
            {
                damage *= 1.5f;
            }

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, i.scale.x / 3, attackables);
            foreach (Collider2D enemy in hitEnemies)
            {
                enemy.GetComponent<EnemyBaseBehavior>().TakeDamage(damage);
            }

            playerAnimator.SetTrigger("loopAttack");
            nextAttackTime = Time.time + 1f / i.attackRate;
            maxAttackCooldown = nextAttackTime - Time.time;
        }
    }

    public void swingAttack(Animator playerAnimator, Item i)
    {
        attackCooldown = nextAttackTime - Time.time;
        if (attackCooldown < 0)
        {
            attackCooldown = 0;
        }
        if (attackCooldown < 0.5f)
        {
            
            bool criticalAttack = false;
            float damage = PlayerStats.instance.damage.getValue();
            if (attackCooldown > 0)
            {
                damage *= (1 - attackCooldown) / 2f;
            } else {
                if (slashParticles != null && giveAttackBonus)
                {
                    var shape = slashParticles.shape;
                    shape.rotation = new Vector3(0f, 0f, player.transform.localScale.x > 0 ? 0f : 90f);

                    slashParticles.Play();
                    damage *= 1.5f;
                    criticalAttack = true;
                }
                
            }

            // Start coroutine to apply delayed damage
            StartCoroutine(DelayedDamage(damage, i));

            // Start ghost trail coroutine
            if (itemHolder != null)
            {
                StartCoroutine(SpawnGhostTrail(i, criticalAttack));
            }

            playerAnimator.SetTrigger("swingAttack");
            nextAttackTime = Time.time + 1f / i.attackRate;
            maxAttackCooldown = nextAttackTime - Time.time;
        }
    }

    private IEnumerator DelayedDamage(float damage, Item i)
    {
        yield return new WaitForSeconds(0.23f);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, i.scale.x / 3, attackables);
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyBaseBehavior>()?.TakeDamage(damage);
        }
        if (slashParticles != null)
        {
            slashParticles.Stop();
        }
    }

    // Spawn ghost trail coroutine
    private IEnumerator SpawnGhostTrail(Item x, bool criticalAttack)
    {
        float duration = 0.23f;
        float timer = 0f;
        for (int i = 0; i < ghostCountPerSwing; i++)
        {
            CreateGhost(x, timer < duration && criticalAttack);
            yield return new WaitForSeconds(ghostSpawnInterval);
            timer += ghostSpawnInterval;
        }
    }

    // Create a ghost copy of the sword sprite at current position and rotation
    private void CreateGhost(Item i, bool addParticles)
    {
        ItemHolderTransformUpdater itemScript = player.GetComponent<ItemHolderTransformUpdater>();

        GameObject ghost = Instantiate(itemHolder.gameObject);
        ghost.name = "SwordGhost";

        // Reparent and reset transform
        ghost.transform.SetParent(player.transform, false);
        ghost.transform.localPosition = Vector3.zero;
        ghost.transform.localRotation = Quaternion.identity;
        ghost.transform.localScale = itemHolder.transform.localScale;
        if (addParticles) {
            ghost.transform.Find("SlashParticles").gameObject.GetComponent<ParticleSystem>().Play();
        }
        
        // Make transparent
        SpriteRenderer ghostRenderer = ghost.GetComponent<SpriteRenderer>();
        ghostRenderer.material = new Material(Shader.Find("Sprites/Default"));
        ghostRenderer.color = new Color(1f, 1f, 1f, 0.3f);

        // Now do everything just like script 1
        Vector3 offset = new Vector3(itemScript.transformX, itemScript.transformY, itemScript.transformZ);
        ghost.transform.localPosition = i.position + offset;

        Vector3 pivot = ghost.transform.position - new Vector3(
            i.scale.x * 1.5f / 10f * player.transform.localScale.x,
            i.scale.y * 1.5f / 10f * player.transform.localScale.y,
            0f);

        ghost.transform.RotateAround(pivot, Vector3.forward, itemScript.rotationAngle * player.transform.localScale.x);


        StartCoroutine(FadeAndDestroy(ghostRenderer, ghostLifetime));
    }






    // Fade out ghost and destroy
    private IEnumerator FadeAndDestroy(SpriteRenderer sr, float time)
    {
        float elapsed = 0f;
        Color originalColor = sr.color;

        while (elapsed < time)
        {
            float alpha = Mathf.Lerp(originalColor.a, 0f, elapsed / time);
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(sr.gameObject);
    }

    void Update()
    {
        attackCooldown = nextAttackTime - Time.time;
        if (attackCooldown < 0)
        {
            attackCooldown = 0;
        }
    }
}
