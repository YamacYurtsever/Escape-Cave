using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishFollow : MonoBehaviour
{
    //Random hareket ederken slow speed olsn diye düşündüm.
    public float fastSpeed = 3f, slowSpeed = 1.2f;
    //Her random hareketten sonra az bi süre duruyor, onun ne kadar olduğunu belirlemek için
    public float randomWaitTimeMin = 1.5f, randomWaitTimeMax = 4f;
    //IsTriggerı açık olan colliderlarla hem fishin içinde hareket ettiği alanı hem de playerın içine girdiği zaman detectleneceği colliderları setliyoruz
    public Collider2D fishMoveCollider;         //Not: Colliderı hareket ettirmek istiyorsan editorde transformdan ettir, yoksa hata olur. colliderın offsetinin 0 0 olması lazım
    public Collider2D playerDetectCollider;

    private GameObject player;
    private PlayerDeath playerDeath;
    private Animator animator;
    private Rigidbody2D rb;
    private Collider2D playerCol;
    private Collider2D fishCol;
    private bool justEntered;
    private Vector2 fishBeforePos;
    private bool beforePosReached = true;
    private bool goingToRandomPoint = false;
    private Vector2 randomPoint;
    private bool canRandomMove = true;
    private float scaleMultiplier;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerDeath = GameObject.FindGameObjectWithTag("PlayerDeath").GetComponent<PlayerDeath>();
        playerCol = player.GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        fishCol = GetComponent<Collider2D>();
        scaleMultiplier = transform.localScale.y;
    }

    void Update()
    {
        if (fishCol.IsTouching(playerCol))
        {
            playerDeath.Death();
        }

        if(playerCol.IsTouching(playerDetectCollider))
        {
            ChasePlayer();
            justEntered = false;
            beforePosReached = false;
        }
        else if(!beforePosReached)
        {
            GoToBeforePos();
            goingToRandomPoint = false;
        }
        else if (canRandomMove)
        {
            justEntered = true;
            MoveRandomly();
        }
    }

    private void ChasePlayer()
    {
        if (justEntered && beforePosReached) fishBeforePos = transform.position;
        
        Vector2 direction = ((Vector2) player.transform.position - (Vector2) transform.position).normalized;
        rb.velocity = direction * fastSpeed;
        transform.localEulerAngles = new Vector3(0, 0, FishEulerAnglesFromDirection(direction));
    }

    private void GoToBeforePos()
    {
        Vector2 direction = (fishBeforePos - (Vector2) transform.position).normalized;
        rb.velocity = direction * fastSpeed;
        transform.localEulerAngles = new Vector3(0, 0, FishEulerAnglesFromDirection(direction));

        if (Vector2.Distance(transform.position, fishBeforePos) < 0.1f)
        {
            beforePosReached = true;
        }
    }

    private void MoveRandomly()
    {
        if(!goingToRandomPoint)
        {
            randomPoint = RandomPointInsideCollider();
            goingToRandomPoint = true;
        }
        Vector2 direction = (randomPoint - (Vector2)transform.position).normalized;
        rb.velocity = direction * slowSpeed;
        transform.localEulerAngles = new Vector3(0, 0, FishEulerAnglesFromDirection(direction));

        if (Vector2.Distance(transform.position, randomPoint) < 0.1f)
        {
            StartCoroutine(WaitBeforeMovingAgain());
        }
    }

    IEnumerator WaitBeforeMovingAgain()
    {
        rb.velocity = Vector2.zero;
        canRandomMove = false;
        goingToRandomPoint = false;
        float delay = Random.Range(randomWaitTimeMin, randomWaitTimeMax);
        yield return new WaitForSeconds(delay);
        canRandomMove = true;
    }

    private float FishEulerAnglesFromDirection(Vector2 direction)
    {
        float angle = Mathf.Rad2Deg * Mathf.Atan(Mathf.Abs(direction.y / direction.x));

        if (direction.x < 0 && direction.y < 0)
        {
            angle += 180;
            transform.localScale = new Vector3(transform.localScale.x, scaleMultiplier * -1, transform.localScale.z);
        }
        if (direction.x > 0 && direction.y < 0)
        {
            angle = 360 - angle;
            transform.localScale = new Vector3(transform.localScale.x, scaleMultiplier, transform.localScale.z);
        }
        if (direction.x < 0 && direction.y > 0)
        { 
            angle = 180 - angle;
            transform.localScale = new Vector3(transform.localScale.x, scaleMultiplier * -1, transform.localScale.z);
        }
        if (direction.x > 0 && direction.y > 0)
        {
            transform.localScale = new Vector3(transform.localScale.x, scaleMultiplier, transform.localScale.z);
        }

        return angle;
    }

    private Vector2 RandomPointInsideCollider()
    {
        float xMin = fishMoveCollider.bounds.min.x;
        float yMin = fishMoveCollider.bounds.min.y;
        float xMax = fishMoveCollider.bounds.max.x;
        float yMax = fishMoveCollider.bounds.max.y;

        float xRandom = Random.Range(xMin, xMax);
        float yRandom = Random.Range(yMin, yMax);

        Vector2 randomPoint = new Vector2(xRandom, yRandom);
        return randomPoint;
    }

    public void SlowDownFish(float index)
    {
        fastSpeed /= index;
        slowSpeed /= index;
        animator.speed = 1 / index;
    }
    public void ReverseSlowDownFish(float index)
    {
        fastSpeed *= index;
        slowSpeed *= index;
        animator.speed = 1;
    }
}
