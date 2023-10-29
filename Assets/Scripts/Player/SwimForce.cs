using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwimForce : MonoBehaviour
{
    public LayerMask waterLayer;
    public float waterGravity;
    public float waterDrag;
    public float swimSpeed;
    public float maxSpeedInWater;
    public PlayerController controller;
    public float gravityOffset;
    public float swimOffset;
    public PlayerAudio playerAudio;
    
    private Rigidbody2D playerRb;
    private Collider2D playerCol;
    private float originalGravity;
    private BreathingSystem breathingSystem;
    private Animator animator;
   
    public bool inWater = false;
    public bool submerged = false;
    public bool touchingWater = false;
    public bool wasInWater = false;
    public bool shocked = false;

    private float horizontalMove, verticalMove;

    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        playerCol = GetComponent<Collider2D>();
        originalGravity = playerRb.gravityScale;
        breathingSystem = GetComponent<BreathingSystem>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal");
        verticalMove = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        //Does player touch water
        wasInWater = inWater;
        inWater = Physics2D.IsTouchingLayers(playerCol, waterLayer.value);

        if (wasInWater && !inWater && breathingSystem.stillHolding)
        {
            if (Input.GetMouseButton(0))
                breathingSystem.SlowDownTime();
            else
                breathingSystem.StartBreathing();

        }

        if (!wasInWater && inWater && breathingSystem.stillHolding)
        {
            breathingSystem.ReverseSlowDownTime();
        }

        if (wasInWater && !inWater)
        {
            playerAudio.SwitchbacktoGroundMusic();
            playerAudio.lighterOn.Play();
        }

        if (!wasInWater && inWater)
        {
            playerAudio.SwitchtoUnderWaterMusic();
            playerAudio.splashAudio.Play();
            playerAudio.splashAudio.time = 0.3f;

            playerAudio.lighterOff.Play();
            playerAudio.WaitAndTurnOffLighterSound();
        }

        breathingSystem.wasInWater = wasInWater;

        //if player touches water
        if (inWater)
        {
            touchingWater = true;

            animator.SetBool("IsSwimming", true);

            if (!breathingSystem.stillHolding)
                breathingSystem.StartHoldingBreath();

            //Set water drag, water gravity, disable normal player movement
            playerRb.gravityScale = waterGravity;
            playerRb.drag = waterDrag;
            controller.enabled = false;
            submerged = true;

            Collider2D waterCol = Physics2D.OverlapCircle((Vector2) transform.position - Vector2.up * gravityOffset , 0.1f, waterLayer);
            if (waterCol == null)
            {
                playerRb.gravityScale = originalGravity;
                playerRb.drag = 0;
                submerged = false;
            }

            //Give ability to swim with force if a good percentage of body is in water
            Collider2D waterCol2 = Physics2D.OverlapCircle((Vector2) transform.position - Vector2.up * swimOffset, 0.2f, waterLayer);
            if (waterCol2 != null)
            {
                Vector2 swimmingForce = new Vector2(horizontalMove, verticalMove) * swimSpeed;
                if(!shocked) playerRb.AddForce(swimmingForce);
            }

            //Get the velocity of player
            float velX = playerRb.velocity.x;
            float vely = playerRb.velocity.y;

            //and clamp it between values so player doesnt accelerate and gain speed and become sonic
            playerRb.velocity = new Vector2(Mathf.Sign(velX) * Mathf.Clamp(Mathf.Abs(velX), 0, maxSpeedInWater), 
                Mathf.Sign(vely) * Mathf.Clamp(Mathf.Abs(vely), 0, maxSpeedInWater));
        }
        else
        {
            //Set gravity, drag, and movement scripts to normal
            playerRb.gravityScale = originalGravity;
            playerRb.drag = 0;
            controller.enabled = true;
            touchingWater = false;
            animator.SetBool("IsSwimming", false);
        }
    }
}
