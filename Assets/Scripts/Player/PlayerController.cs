using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
	[Range(0, .3f)] public float movementSoothing = 0.05f;
	public float wallSlideSpeed = 0f;
	public float wallJumpTime = 0f;
	public float wallDistance = 0.5f;
	public float wallJumpForceY = 500f;
	public float wallJumpForceX = 400f;
	public float sideWallJumpForceX = 750f;
	public float jumpWaiter = 0.25f;
	public bool isGrounded;
	public bool isWallSliding;
	public LayerMask groundLayers;
	public GameObject dustParticle;
	public Transform groundCheck;

	private bool wallCheckHit1, wallCheckHit2;
	private Rigidbody2D rb;
	private Vector3 velocity = Vector3.zero;
	private PlayerMovement playerMovement;
	private float jumpTime;
	private float jumpCounter = 0f;
	private Animator animator;
	private bool canJump = true;
    private SwimForce swimForce;


	//events
	[Header("Events")]
	[Space]
	public UnityEvent OnLandEvent;
	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
	    playerMovement = GetComponent<PlayerMovement>();
        swimForce = GetComponent<SwimForce>();

        if (OnLandEvent == null)
		{
			OnLandEvent = new UnityEvent();
		}
	}

	private void Update()
    {
        bool wasGrounded = isGrounded;
        isGrounded = false;

        //apply land
        if (Physics2D.OverlapPoint(groundCheck.position, groundLayers))
        {
            isGrounded = true;
            animator.SetBool("IsSliding", false);
            jumpTime = Time.time + wallJumpTime;
            if (!wasGrounded)
                OnLandEvent.Invoke();
        }

        WallSlide();
    }

    private void WallSlide()
    {
        //apply wall slide and jump
        wallCheckHit1 = Physics2D.OverlapPoint(new Vector2(transform.position.x + wallDistance, transform.position.y - 0.5f), groundLayers);
        wallCheckHit2 = Physics2D.OverlapPoint(new Vector2(transform.position.x - wallDistance, transform.position.y - 0.5f), groundLayers);

        if (wallCheckHit1 && !isGrounded && playerMovement.horizontalMove > 0f && !swimForce.inWater)
        {
            isWallSliding = true;
            Vector3 theScale = transform.localScale;
            theScale.x = -1;
            transform.localScale = theScale;
            animator.SetBool("IsSliding", true);
            jumpTime = Time.time + wallJumpTime;
        }
        else if (wallCheckHit2 && !isGrounded && playerMovement.horizontalMove < 0f && !swimForce.inWater)
        {
            isWallSliding = true;
            Vector3 theScale = transform.localScale;
            theScale.x = 1;
            transform.localScale = theScale;
            animator.SetBool("IsSliding", true);
            jumpTime = Time.time + wallJumpTime;
        }
        else if (jumpTime < Time.time)
        {
            animator.SetBool("IsSliding", false);
            if (isWallSliding == true)
            {
                isWallSliding = false;
            }
        }

        if (isWallSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlideSpeed, float.MaxValue));

            if (Input.GetButtonDown("Jump") && jumpCounter < 3f && canJump)
            {
                if (playerMovement.horizontalMove > 0 && transform.localScale.x == -1)
                {
                    rb.velocity = new Vector2(0, 0);
                    rb.AddForce(new Vector2(-wallJumpForceX, wallJumpForceY));
                }
                else if (playerMovement.horizontalMove < 0 && transform.localScale.x == 1)
                {
                    rb.velocity = new Vector2(0, 0);
                    rb.AddForce(new Vector2(wallJumpForceX, wallJumpForceY));
                }

                Instantiate(dustParticle, new Vector2(transform.position.x, transform.position.y - 0.9f), transform.rotation);
                jumpCounter++;
                animator.SetBool("IsJumping", true);
            }
        }

        if (isGrounded)
        {
            jumpCounter = 0f;
        }

        if (swimForce.inWater)
        {
            animator.SetBool("IsSliding", false);
            animator.SetBool("IsSwimming", true);
            isWallSliding = false;
        }
    }

    public void Move(float move)
	{
		//apply move
		Vector3 targetVelocity = new Vector2(move, rb.velocity.y);
		rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSoothing);
	}

	public void Jump(bool jump, float jumpForce)
	{
		//apply jump
		if (isGrounded && jump && !isWallSliding && canJump)
		{
			rb.velocity = new Vector2(rb.velocity.x, 0f);
			rb.AddForce(new Vector2(0f, jumpForce));
			Instantiate(dustParticle, new Vector2(transform.position.x, transform.position.y - 0.9f), transform.rotation);
		}
	}
}