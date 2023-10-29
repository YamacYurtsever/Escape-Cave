using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundLava : MonoBehaviour
{
    public float disappearCooldown = 2.5f;

    private GameObject player;
    private Collider2D playerCol;
    private PlayerDeath playerDeath;
    private Collider2D thisCol;
    private Animator animator;
    private float timeMultiplier = 1;
    private float timer = 0;

    void Start()
    {
        animator = GetComponent<Animator>();
        thisCol = GetComponent<Collider2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerDeath = GameObject.FindGameObjectWithTag("PlayerDeath").GetComponent<PlayerDeath>();
        playerCol = player.GetComponent<Collider2D>();
    }


    void FixedUpdate()
    {
        if(thisCol.IsTouching(playerCol))
        {
            playerDeath.Death();
        }

        timer += Time.fixedDeltaTime * timeMultiplier;
        if(timer > disappearCooldown)
        {
            Destroy(gameObject);
        }
    }

    public void SlowDown(float index)
    {
        timeMultiplier /= index;
        animator.speed = 1 / index;
    }
    public void ReverseSlowDown(float index)
    {
        timeMultiplier *= index;
        animator.speed = 1;
    }
}
