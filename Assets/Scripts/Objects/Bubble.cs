using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    public float bubbleBreathIncrease = 0.2f;
    public LayerMask waterLayer;
    public float speed = 1.5f;

    private GameObject player;
    private Collider2D playerCol;
    private Collider2D bubbleCol;
    private Rigidbody2D bubbleRb;
    private Animator bubbleAnim;
    private BreathingSystem breathSystem;
    private int waterLayerValue;
    private Vector2 surfacePoint;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerCol = player.GetComponent<Collider2D>();
        breathSystem = player.GetComponent<BreathingSystem>();
        bubbleCol = GetComponent<Collider2D>();
        bubbleRb = GetComponent<Rigidbody2D>();
        bubbleAnim = GetComponent<Animator>();
        waterLayerValue = waterLayer.value;

        bool x = true;
        Collider2D col;
        int count = -1;
        while(x)
        {
            count++;
            col = Physics2D.OverlapPoint((Vector2)transform.position + Vector2.up * count * 0.1f, waterLayerValue);
            if (col == null) x = false;
        }
        surfacePoint = (Vector2)transform.position + Vector2.up * count * 0.1f;
    }

    void FixedUpdate()
    {
        bubbleRb.velocity = Vector2.up * speed;
        CheckCollisionWithPlayer();
        CheckCollisionWithSurface();
    }

    public void OpenConstraints()
    {
        bubbleRb.constraints = RigidbodyConstraints2D.None;
    }
    
    private void CheckCollisionWithPlayer()
    {
        if (bubbleCol.IsTouching(playerCol))
        {
            //increase breath
            breathSystem.IncreaseBreath(bubbleBreathIncrease);

            //burst into smaller decoration bubbles
            bubbleRb.constraints = RigidbodyConstraints2D.FreezeAll;
            Destroyer();
        }
    }

    private void CheckCollisionWithSurface()
    {
        if (transform.position.y > surfacePoint.y - 0.25f)
        {
            //suya artık değmiyo surface ta
            //animasyon vesayre olabilir
            bubbleRb.constraints = RigidbodyConstraints2D.FreezeAll;
            bubbleAnim.SetTrigger("Pop");
        }
    }

    public void Destroyer()
    {
        Destroy(gameObject);
    }

    public void SlowDownBubble(float index)
    {
        bubbleRb.velocity /= index;
        bubbleAnim.speed = 1 / index;
    }
    public void ReverseSlowDownBubble(float index)
    {
        bubbleRb.velocity *= index;
        bubbleAnim.speed = 1;
    }
}
