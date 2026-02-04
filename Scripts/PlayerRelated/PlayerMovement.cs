using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [HideInInspector] public PlayerController controller;
    [HideInInspector] public Animator animator;

    float horizontalMove = 0f;
    public float runSpeed = 40f;
    bool jump = false;
    [HideInInspector] public int x = 1;

    void Awake()
    {
        // Automatically get the components on the same GameObject
        if (controller == null)
            controller = GetComponent<PlayerController>();

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    public void Land()
    {
        if (x <= 0)
        {
            animator.SetBool("isJumping", false);
        }
        else
        {
            x -= 1;
        }
    }

    void Update()
    {
        float playerSpeed = PlayerStats.instance.speed.getValue();
        if (playerSpeed==0){
            playerSpeed=1;
        }
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed * playerSpeed;

        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.W))
        {
            jump = true;
            animator.SetBool("isJumping", true);
            x = 1;
        }
    }

    void FixedUpdate()
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, false, jump);
        jump = false;
    }
}
