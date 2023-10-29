using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snail : MonoBehaviour
{
    public float movementRangeLeft, movementRangeRight;
    public LayerMask groundLayer;
    public float speed;
    public Vector2 overlapCircleOffset;

    private PlayerDeath playerDeath;
    private GameObject player;
    private Collider2D playerCol;
    private Collider2D snailCol;
    private float startPosX;
    private Rigidbody2D rb;
    private bool movingRight = true;
    private float initialScale;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerDeath = GameObject.FindGameObjectWithTag("PlayerDeath").GetComponent<PlayerDeath>();
        playerCol = player.GetComponent<Collider2D>();
        startPosX = transform.position.x;
        rb = GetComponent<Rigidbody2D>();
        snailCol = GetComponent<Collider2D>();
        initialScale = transform.localScale.x;
    }

    void Update()
    {
        if(transform.position.x > (startPosX + movementRangeRight))
        {
            movingRight = false;
        }
        else if (transform.position.x < (startPosX - movementRangeLeft))
        {
            movingRight = true;
        }

        if (movingRight) MoveRight();
        else MoveLeft();

        if(snailCol.IsTouching(playerCol))
        {
            playerDeath.Death();
        }
    }

    private void MoveRight()
    {
        Collider2D upRightCol = Physics2D.OverlapCircle( (Vector2) transform.position + overlapCircleOffset, 0.1f, groundLayer.value);
        Collider2D downRightCol = Physics2D.OverlapCircle((Vector2)transform.position +  new Vector2(overlapCircleOffset.x, -1 * overlapCircleOffset.y),
            0.1f, groundLayer.value);

        transform.localScale = new Vector3(initialScale * -1, transform.localScale.y, transform.localScale.z);

        if (upRightCol != null)
        {
            rb.velocity = Vector2.up * speed;
        }
        else if (downRightCol == null)
        {
            rb.velocity = Vector2.down * speed;
        }
        else
        {
            rb.velocity = Vector2.right * speed;
        }
    }

    private void MoveLeft()
    {
        Collider2D upLeftCol = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(-1 * overlapCircleOffset.x, overlapCircleOffset.y), 
            0.1f, groundLayer.value);
        Collider2D downLeftCol = Physics2D.OverlapCircle((Vector2)transform.position - overlapCircleOffset, 0.1f, groundLayer.value);

        transform.localScale = new Vector3(initialScale, transform.localScale.y, transform.localScale.z);

        if (upLeftCol != null)
        {
            rb.velocity = Vector2.up * speed;
        }
        else if (downLeftCol == null)
        {
            rb.velocity = Vector2.down * speed;
        }
        else
        {
            rb.velocity = Vector2.left * speed;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.position + new Vector3(overlapCircleOffset.x, overlapCircleOffset.y, 0),0.1f);
    }

    public void SlowDownSnail(float index)
    {
        speed /= index;
    }
    public void ReverseSlowDownSnail(float index)
    {
        speed *= index;
    }
}