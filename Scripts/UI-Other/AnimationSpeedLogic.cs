using UnityEngine;

public class AnimationSpeedLogic : MonoBehaviour
{
    // Reference to the Animator component (assign via Inspector or automatically found)
    public Animator animator;

    // Public variable to control the animation speed. 1.0 is normal speed.
    public float animationSpeed = 1f;

    private void Awake()
    {
        // Auto-assign the Animator if not set in the Inspector.
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void Update()
    {
        // Update the Animator's speed to match the public variable.
        animator.speed = animationSpeed;
    }
}
