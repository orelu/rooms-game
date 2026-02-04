using UnityEngine;
using Pathfinding;
using System.Collections;

public class RockEnemyLogic : MonoBehaviour
{
    [Header("Pathfinding")]
    public Transform target;
    public float activateDistance = 8f;
    public float pathUpdateSeconds = 0.5f;

    [Header("Physics")]
    public float speed = 200f;
    public float nextWayPointDistance = 3f;
    public float jumpNodeHeightRequirement = 0.8f;
    public float jumpModifier = 0.3f;
    public float jumpCheckOffset = 0.1f;

    [Header("Custom Behavior")]
    public bool followEnabled = true;
    public bool jumpEnabled = true;
    public bool directionLookEnabled = true;

    public float attackSeconds = 1f;
    public float attackDistance = 0.5f;

    private Path path;
    private int currentWaypoint = 0;
    private bool isGrounded = false;
    private Seeker seeker;
    private Rigidbody2D rb;
    private Collider2D coll;

    public Animator animator;

    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();

        InvokeRepeating(nameof(UpdatePath), 0f, pathUpdateSeconds);
        InvokeRepeating(nameof(AttackPlayer), 0f, attackSeconds);
    }

    void FixedUpdate()
    {
        if (TargetInDistance() && followEnabled)
        {
            PathFollow();
        }

        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
    }

    void UpdatePath()
    {
        if (followEnabled && TargetInDistance() && seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    void PathFollow()
    {
        if (path == null) return;

        if (currentWaypoint >= path.vectorPath.Count)
        {
            // Final direct movement toward the target if path is done
            Vector2 directionToTarget = ((Vector2)target.position - rb.position).normalized;
            Vector2 forceToTarget = directionToTarget * speed * Time.deltaTime;
            rb.AddForce(forceToTarget);
            return;
        }

        // Check if grounded
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, coll.bounds.extents.y + jumpCheckOffset);

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        if (jumpEnabled && isGrounded && direction.y > jumpNodeHeightRequirement)
        {
            rb.AddForce(Vector2.up * speed * jumpModifier);
        }

        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWayPointDistance)
        {
            currentWaypoint++;
        }

        if (directionLookEnabled)
        {
            if (rb.velocity.x > 0.05f)
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (rb.velocity.x < -0.05f)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
    }


    bool TargetInDistance()
    {
        return Vector2.Distance(transform.position, target.position) < activateDistance;
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        } else {
            Debug.Log("error!");
        }
    }

    void AttackPlayer() {
        GetComponent<EnemyBaseBehavior>().AttackPlayer(10,attackDistance);
    }
}
