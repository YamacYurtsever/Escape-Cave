using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float runSpeed = 100f;
    public float jumpForce = 400f;
    public float horizontalMove = 0f;
    public float breathCooldown = 1f;
    public float breathTimer = 0;

    private PlayerDeath playerDeath;
    private PlayerController controller;
    private Animator animator;
    private bool jump = false;
    private BreathingSystem breathingSystem;
    private SwimForce swimForce;

    private void Start()
    {
        controller = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        playerDeath = GameObject.FindGameObjectWithTag("PlayerDeath").GetComponent<PlayerDeath>();
        breathingSystem = GetComponent<BreathingSystem>();
        swimForce = GetComponent<SwimForce>();
    }

    private void Update()
    {
        if (!swimForce.inWater)
        {
            //input move
            horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
            animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

            //input jump
            if (Input.GetButtonDown("Jump"))
            {
                jump = true;
                animator.SetBool("IsJumping", true);
            }

            //input breath
            //Put Colldown If The Player Just Came Out of the Water
            if (swimForce.wasInWater)
                breathTimer = Time.time + breathCooldown;
            if (Input.GetMouseButton(0) && Time.time > breathTimer && !breathingSystem.stillHolding && !swimForce.inWater)
            {
                breathingSystem.StartHoldingBreath();
                breathTimer = Time.time + breathCooldown;
            }
            else if ((Input.GetMouseButtonUp(0) && breathingSystem.stillHolding && !swimForce.inWater))
            {
                breathingSystem.StartBreathing();
            }
        }
    }

    private void FixedUpdate()
    {
        if (!swimForce.inWater)
        {
            //apply move
            controller.Move(horizontalMove * Time.fixedDeltaTime);
            controller.Jump(jump, jumpForce);

            //stop jump
            jump = false;
        }
    }

    public void OnLanding()
    {
        //apply land
        animator.SetBool("IsJumping", false);
        controller.isWallSliding = false;
    }

    public void PlayerDies()
    {
        playerDeath.PlayerSpriteDisappear();
    }
}