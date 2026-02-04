using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBaseBehavior : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public float health = 100;
    public Stat healthRegeneration;
    public Stat armor;

    [Header("Combat Settings")]
    public Stat damage;
    public Stat speed;
    public float knockbackStrength = 5f;
    public float disableAirControlLength = 0.5f;

    [Header("XP Drop Settings")]
    [Range(0f, 1f), Tooltip("Chance between 0 and 1 to drop XP instead of an item.")]
    public float xpDropChance = 0.5f;
    public int low_xp_drop = 1;
    public int high_xp_drop = 5;

    [Header("Item Drop Settings")]
    public Item drop;

    [Header("Death Visuals")]
    public Sprite deathSprite;
    public Material deathSpriteMaterial;

    [Header("UI References")]
    public BarScript healthBar;

    // Private
    private LayerMask players;
    private int regenCounter = 250;

    void Start()
    {
        health = maxHealth;
        players = LayerMask.GetMask("Players"); // Always target objects on "Players" layer
    }

    void FixedUpdate()
    {
        if (regenCounter <= 0)
        {
            health += healthRegeneration.getValue();
            regenCounter = 250;
            if (health > maxHealth) { health = maxHealth; }
        }
        else
        {
            regenCounter--;
        }
    }

    public void TakeDamage(float damage)
    {
        damage -= armor.getValue();
        damage = Mathf.Clamp(damage, 0, int.MaxValue);
        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    void OnMouseEnter() => ItemLogic.instance.giveAttackBonus = true;
    void OnMouseExit() => ItemLogic.instance.giveAttackBonus = false;

    public void AttackPlayer(int damage, float distance)
    {
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(transform.position, distance, players);
        foreach (Collider2D player in hitPlayer)
        {
            if (player.gameObject == gameObject) continue;

            PlayerStats.instance.TakeDamage(damage);

            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                float horizontalDir = Mathf.Sign(player.transform.position.x - transform.position.x);
                float angleInRadians = 30f * Mathf.Deg2Rad;
                Vector2 knockbackDir = new Vector2(
                    Mathf.Cos(angleInRadians) * horizontalDir,
                    Mathf.Sin(angleInRadians)
                ).normalized;

                Vector2 velocity = playerRb.velocity;
                if (velocity.y < 0) velocity.y = 0f;
                playerRb.velocity = velocity;

                // Check if player is touching the ground
                bool isGrounded = player.IsTouchingLayers(LayerMask.GetMask("Ground"));

                // Scale knockback if airborne
                float knockbackScale = isGrounded ? 1f : 0.5f;

                Debug.DrawRay(player.transform.position, knockbackDir * 2f, Color.red, 1f);
                playerRb.AddForce(knockbackDir * knockbackStrength * knockbackScale, ForceMode2D.Impulse);

                PlayerController controller = player.GetComponent<PlayerController>();
                if (controller != null)
                {
                    controller.StartCoroutine(DisableAirControlTemporarily(controller, disableAirControlLength));
                }
            }
        }
    }

    

    public void dropItem(int xpAmount, Item drop)
    {
        // Decide object name
        string objName = (drop != null) ? $"Dropped Item: {drop.name}" : "Dropped XP";

        // Create the GameObject
        GameObject droppedItem = new GameObject(objName);

        // Add components once
        CircleCollider2D collider = droppedItem.AddComponent<CircleCollider2D>();
        SpriteRenderer sr = droppedItem.AddComponent<SpriteRenderer>();
        DroppedItemProporties props = droppedItem.AddComponent<DroppedItemProporties>();

        // Assign shared properties
        props.players = players;
        droppedItem.transform.position = transform.position;

        droppedItem.layer = LayerMask.NameToLayer("Dropped");

        int xpLayer = droppedItem.layer;
        int attackableLayer = LayerMask.NameToLayer("Attackable");
        Physics2D.IgnoreLayerCollision(xpLayer, attackableLayer, true);


        // XP orb or normal item
        if (xpAmount > 0)
        {
            props.isXPOrb = true;
            props.xp = xpAmount;
            sr.sprite = Resources.Load<Sprite>("xp");
        }
        else if (drop != null)
        {
            props.item = drop;
            sr.sprite = drop.icon;
        }
    }


    public virtual void Die()
    {
        float roll = Random.value; // Random between 0 and 1

        if (drop==null || roll <= xpDropChance)
        {
            int xpAmount = Random.Range(low_xp_drop, high_xp_drop + 1);
            dropItem(xpAmount, null);
        }
        else
        {
            dropItem(0, drop);
        }

        ItemLogic.instance.giveAttackBonus = false;

        GameObject deathBeam = new GameObject("DeathBeam");
        deathBeam.AddComponent<SpriteRenderer>();
        deathBeam.AddComponent<MoveDown>();
        deathBeam.GetComponent<SpriteRenderer>().sprite = deathSprite;
        deathBeam.GetComponent<SpriteRenderer>().material = deathSpriteMaterial;
        deathBeam.transform.position = new Vector3(transform.position.x, transform.position.y + 15, transform.position.z);
        deathBeam.transform.localScale = new Vector3(5, 100, 1);

        Destroy(deathBeam, 1f);
        Destroy(gameObject);
    }

    private IEnumerator DisableAirControlTemporarily(PlayerController controller, float duration)
    {
        controller.m_AirControl = false;
        yield return new WaitForSeconds(duration);
        controller.m_AirControl = true;
    }
}
