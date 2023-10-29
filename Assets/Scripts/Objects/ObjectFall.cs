using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFall : MonoBehaviour
{
    public LayerMask groundLayer;
    public float fallSpeed = 7f;
    public float fallGravity;
    public float delay;
    public float horizontalRangeforActivation;
    public float verticalRangeforActivation;
    public float delayBetweenNearObjects = 0.25f;
    public bool isFalling = false;
    public bool trigerred = false;
    public LayerMask stalactiteLayer;
    public AudioSource shake, breaking;

    private PlayerDeath playerDeath;
    private GameObject player;
    private Collider2D playerCol;
    private Animator animator;
    private Rigidbody2D rb;
    private Collider2D objectCol;
    private Vector2 objectPos;
    private Collider2D leftStalactite, rightStalactite;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerDeath = GameObject.FindGameObjectWithTag("PlayerDeath").GetComponent<PlayerDeath>();
        playerCol = player.GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        objectPos = transform.position;
        objectCol = GetComponent<Collider2D>();
    }

    void Update()
    {
        //Checks if player is within object's range
        Vector2 playerPos = player.transform.position;
        if(playerPos.x > (objectPos.x - horizontalRangeforActivation) &&
           playerPos.x < (objectPos.x + horizontalRangeforActivation) &&
           playerPos.y > (objectPos.y - verticalRangeforActivation)   &&
           playerPos.y < (objectPos.y + verticalRangeforActivation)   && 
           !isFalling)
        {
            isFalling = true;
            animator.SetTrigger("Fall");
            shake.Play();
        }

        if (objectCol.IsTouching(playerCol))
        {
            playerDeath.Death();
            Destroy(gameObject);
        }
        else if (objectCol.IsTouchingLayers(groundLayer.value))
        {
            animator.SetTrigger("Destroy");
            breaking.Play();
            objectCol.enabled = false;
            transform.position = new Vector3(transform.position.x, transform.position.y - 0.2f, transform.position.z);
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        if(trigerred && !isFalling)
        {
            StartCoroutine(FallWithDelay());
        }
    }


    public void StartFall()
    {
        //A customization to make the fall both with gravity and with constant velocity
        StartCoroutine(DelayAccelerateAndFall());
    }

    IEnumerator DelayAccelerateAndFall()
    {
        //Delay (starts after animation ends) and set gravity
        StartCoroutine(Shake());
        yield return new WaitForSeconds(delay);
        shake.Stop();
        rb.gravityScale = fallGravity;
        StartCoroutine(ClampFallVelocity());
    }

    public void TriggerNearStalactites()
    {
        leftStalactite = Physics2D.OverlapArea(new Vector2(transform.position.x - 1f, transform.position.y + 1),
                                       new Vector2(transform.position.x - 0.5f, transform.position.y - 1),
                                       stalactiteLayer);
        rightStalactite = Physics2D.OverlapArea(new Vector2(transform.position.x + 1f, transform.position.y + 1),
                                                new Vector2(transform.position.x + 0.5f, transform.position.y - 1),
                                                stalactiteLayer);

        if (leftStalactite != null)
            leftStalactite.GetComponent<ObjectFall>().trigerred = true;
        if (rightStalactite != null)
            rightStalactite.GetComponent<ObjectFall>().trigerred = true;
    }

    IEnumerator FallWithDelay()
    {
        yield return new WaitForSeconds(delayBetweenNearObjects);
        isFalling = true;
        animator.SetTrigger("Fall");
    }

    IEnumerator Shake()
    {
        transform.position = new Vector2(transform.position.x + 0.1f, transform.position.y);
        yield return new WaitForSeconds(delay / 2);
        transform.position = new Vector2(transform.position.x - 0.2f, transform.position.y);
        yield return new WaitForSeconds(delay / 2);
        transform.position = new Vector2(transform.position.x + 0.1f, transform.position.y);
    }

    IEnumerator ClampFallVelocity()
    {
        //Gets velocity value and clamps it. Max value the vertical velocity will have is fallSpeed
        float yVel = rb.velocity.y;
        rb.velocity = new Vector2(0, -1 * Mathf.Clamp(Mathf.Abs(yVel), 0, fallSpeed));
        
        //Loop, broken by being destroyed
        yield return new WaitForSeconds(0.02f);
        yield return StartCoroutine(ClampFallVelocity());
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public void SlowDownFall(float index)
    {
        fallSpeed /= index;
        animator.speed = 1 / index;
    }
    public void ReverseSlowDownFall(float index)
    {
        fallSpeed *= index;
        animator.speed = 1;
    }
}
