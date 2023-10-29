using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishPatrol : MonoBehaviour
{
    public bool horizontalNotVertical = false;
    public float patrolUnitPositive = 1.3f, patrolUnitNegative = 3.3f;
    public float speed = 2.6f;

    private GameObject player;
    private PlayerDeath playerDeath;
    private Animator animator;
    private Rigidbody2D rb;
    private Collider2D playerCol;
    private Collider2D fishCol;
    private Vector2 patrolPointPositive, patrolPointNegative;
    private Vector2 dir;
    private bool goingPositive = true, goingNegative = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerDeath = GameObject.FindGameObjectWithTag("PlayerDeath").GetComponent<PlayerDeath>();
        playerCol = player.GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        fishCol = GetComponent<Collider2D>();

        if(horizontalNotVertical)
        {
            patrolPointPositive = (Vector2)transform.position + new Vector2(patrolUnitPositive, 0);
            patrolPointNegative = (Vector2)transform.position + new Vector2(-patrolUnitNegative, 0);
            dir = Vector2.right;
        }
        else if (!horizontalNotVertical)
        {
            patrolPointPositive = (Vector2)transform.position + new Vector2(0, patrolUnitPositive);
            patrolPointNegative = (Vector2)transform.position + new Vector2(0, -patrolUnitNegative);
            dir = Vector2.up;
        }
    }

    void Update()
    {
        if(fishCol.IsTouching(playerCol))
        {
            playerDeath.Death();
        }

        Patrol();
    }

    private void Patrol()
    {
        rb.velocity = dir * speed;

        if (Vector2.Distance(transform.position, patrolPointPositive) < 0.1f && goingPositive)
        {
            dir *= -1;
            transform.eulerAngles = new Vector3(0,0,-90);
            goingPositive = false;
            goingNegative = true;
        } 
        
        if (Vector2.Distance(transform.position, patrolPointNegative) < 0.1f && goingNegative)
        {
            dir *= -1;
            transform.eulerAngles = new Vector3(0, 0, 90);
            goingNegative = false;
            goingPositive = true;
        }
    }

    public void SlowDownFish(float index)
    {
        speed /= index;
        animator.speed = 1 / index;
    }
    public void ReverseSlowDownFish(float index)
    {
        speed *= index;
        animator.speed = 1;
    }
}

