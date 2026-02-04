using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLogic : MonoBehaviour
{
    public LayerMask players;
    public LayerMask ground;
    public int damage = 5;
    public float distance = 0.1f;
    public bool willRotate = true;

    public int rotationSpeed = 5;

    void Start()
    {
        // Automatically assign LayerMasks by name
        players = LayerMask.GetMask("Players");
        ground = LayerMask.GetMask("Ground");
    }

    void Update()
    {
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(transform.position, distance, players);
        foreach (Collider2D player in hitPlayer)
        {
            PlayerStats.instance.TakeDamage(damage);
            Destroy(gameObject);
        }

        Collider2D[] hitGround = Physics2D.OverlapCircleAll(transform.position, distance, ground);
        foreach (Collider2D g in hitGround)
        {
            Destroy(gameObject);
        }

        if (willRotate)
        {
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime * 100);
        }
    }
}
