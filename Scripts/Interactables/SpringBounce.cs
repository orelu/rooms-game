using UnityEngine;
using System.Collections;

public class SpringBounce : MonoBehaviour
{
    public float bounceForce = 30f;
    public float disableAirControlLength = 0f;

    // Choose direction in inspector or via code (e.g., (0,1)=up, (1,0)=right, (-1,0)=left, (0,-1)=down)
    public Vector2 bounceDirection = Vector2.up;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Normalize to ensure consistent force direction
                Vector2 dir = bounceDirection.normalized;

                // Cancel velocity in the direction of bounce to avoid resistance
                Vector2 velocity = rb.velocity;
                float projectedSpeed = Vector2.Dot(velocity, dir);
                velocity -= dir * projectedSpeed;
                rb.velocity = velocity;

                // Apply bounce
                rb.AddForce(dir * bounceForce, ForceMode2D.Impulse);

                
            }

            PlayerController controller = other.GetComponent<PlayerController>();

            if (controller != null)
            {
                controller.StartCoroutine(DisableAirControlTemporarily(controller, disableAirControlLength));
            }
        }

        
    }

    private IEnumerator DisableAirControlTemporarily(PlayerController controller, float duration)
    {
        controller.m_AirControl = false;
        yield return new WaitForSeconds(duration);
        controller.m_AirControl = true;
    }
}
