using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropDestroy : MonoBehaviour
{
    public float fallSpeed;
    public float fallGravity;
    public LayerMask groundLayer;

    private Rigidbody2D dropRb;
    private Collider2D dropCol;
    private Animator animator;
    private DripAudio dripAudio;
    private SpriteRenderer sr;

    private void Start()
    {
        dropRb = GetComponent<Rigidbody2D>();
        dropCol = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        dripAudio = GetComponent<DripAudio>();
        sr = GetComponent<SpriteRenderer>();
    }

    public void StartFallingFromAnimation()
    {
        //Delay (starts after animation ends) and set gravity
        dropRb.gravityScale = fallGravity;
        StartCoroutine(ClampFallVelocity());

        //Destroy when object hits ground
        StartCoroutine(CheckIfGroundHit());
    }

    IEnumerator ClampFallVelocity()
    {
        //Gets velocity value and clamps it. Max value the vertical velocity will have is fallSpeed
        float yVel = dropRb.velocity.y;
        dropRb.velocity = new Vector2(0, -1 * Mathf.Clamp(Mathf.Abs(yVel), 0, fallSpeed));

        //Loop, broken by being destroyed
        yield return new WaitForSeconds(0.02f);
        yield return StartCoroutine(ClampFallVelocity());
    }

    IEnumerator CheckIfGroundHit()
    {
        //Is drop touching ground, if so splash on floor
        bool isTouchingGround = Physics2D.IsTouchingLayers(dropCol, groundLayer.value);
        if (isTouchingGround) DropSplash();

        //Infinite loop, object being destroyed breaks loop
        if (!isTouchingGround)
        {
            yield return new WaitForSeconds(0.02f);
            yield return StartCoroutine(CheckIfGroundHit());
        }
    }

    private void DropSplash()
    {
        StopAllCoroutines();
        //splash animation
        dripAudio.PlayDripNoise();
        GetComponent<Animator>().SetTrigger("Splash");
        dropRb.bodyType = RigidbodyType2D.Static;
    }

    public void DestroyOnAnimationEnd()
    {
        sr.enabled = false;
        dropCol.enabled = false;
        dropRb.bodyType = RigidbodyType2D.Static;
        StartCoroutine(DestroyAfterTime());
    }

    IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(0.4f);
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
