using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyFish : MonoBehaviour
{
    public float shockTime;
    public float force;
    public float drag;
    public int moveAmountUp = 1, moveAmountDown = 1;
    public bool rightUpnotLeftup = true;
    public float initialDelay = 1f, cooldown = 2f;

    private GameObject player;
    private Collider2D playerCol;
    private Animator playerAnim;
    private Ligther playerLigther;
    private Collider2D jellyfishCol;
    private Rigidbody2D jellyfishRb;
    private Animator animator;
    private SwimForce swimScript;
    private bool alreadyStarted = false;
    private bool touchedJellyfish = false;
    private Vector2 direction;
    private int moveCount = 0;
    private bool movingUp = true;
    private float forceMultiplier = 1;
    private float timeMultiplier = 1;
    private float timer = 0;
    private float rotation1, rotation2;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        swimScript = player.GetComponent<SwimForce>();
        playerCol = player.GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        jellyfishRb = GetComponent<Rigidbody2D>();
        jellyfishCol = GetComponent<Collider2D>();
        playerAnim = player.GetComponent<Animator>();
        playerLigther = player.GetComponent<Ligther>();

        jellyfishRb.drag = drag;
        if (rightUpnotLeftup)
        {
            direction = new Vector2(1, 1);
            rotation1 = 315;
            rotation2 = 135;
        }
        else
        {
            direction = new Vector2(-1, 1);
            rotation1 = 45;
            rotation2 = 225;
        }

        transform.eulerAngles = new Vector3(0, 0, rotation1);
            
        direction = direction.normalized;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timer += Time.fixedDeltaTime * timeMultiplier;
        if (timer > cooldown + initialDelay)
        {
            //if so, starts object animation
            JellyFishMove();
            initialDelay = 0;
            timer = 0;
        }

        PlayerShockControl();
    }

    private void JellyFishMove()
    {
        if(moveCount == moveAmountUp)
        {
            direction *= -1;
            transform.eulerAngles = new Vector3(0, 0, rotation2);
            movingUp = false;
        }
        else if(moveCount == -1 * moveAmountDown)
        {
            direction *= -1;
            transform.eulerAngles = new Vector3(0, 0, rotation1);
            movingUp = true;
        }

        if (movingUp) moveCount++;
        else moveCount--;

        jellyfishRb.AddForce(direction * force * forceMultiplier, ForceMode2D.Impulse);
    }

    private void PlayerShockControl()
    {
        if (jellyfishCol.IsTouching(playerCol) && !playerLigther.isStunned)
        {
            swimScript.shocked = true;
            touchedJellyfish = true;
            playerAnim.SetBool("IsStunned", true);
            playerLigther.newLocationX1 = player.transform.position.x + 0.5f;
            playerLigther.newLocationX2 = player.transform.position.x - 0.5f;
            playerLigther.isStunned = true;
        }
        if (!jellyfishCol.IsTouching(playerCol) && touchedJellyfish && swimScript.shocked && !alreadyStarted)
        {
            touchedJellyfish = false;
            StartCoroutine(EnableSwimAfterWait());
        }
    }

    IEnumerator EnableSwimAfterWait()
    {
        alreadyStarted = true;
        yield return new WaitForSeconds(shockTime);
        playerAnim.SetBool("IsStunned", false);
        swimScript.shocked = false;
        playerLigther.isStunned = false;
        alreadyStarted = false;
    }

    public void SlowDownJellyfish(float index)
    {
        jellyfishRb.velocity /= index;
        jellyfishRb.drag /= (index * index);
        animator.speed = 1 / index;
        forceMultiplier /= index;
        timeMultiplier /= index;
    }
    public void ReverseSlowDownJellyfish(float index)
    {
        jellyfishRb.velocity *= index;
        jellyfishRb.drag *= (index * index);
        animator.speed = 1;
        forceMultiplier = 1;
        timeMultiplier = 1;
    }
}
