using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleSpawn : MonoBehaviour
{
    public float speed = 1.5f;
    public GameObject bubblePrefab;
    public Transform bubbleStorage;
    public float minSmallBubbleCooldown = 0.06f, maxSmallBubbleCooldown = 0.12f;
    public int minAmountBubble = 2, maxAmountBubble = 4;
    public float bubblePositionMaxDifference = 0.8f;
    public float spawnCooldown = 5f;
    public float spawnDistance = 1f;
    public float initialDelay = 0f;

    private GameObject bubble;
    private float timer = 0;
    private float timeMultiplier = 1f;
    private Animator animator;
    private int bubbleCount = 0;

    void Start()
    {
        animator = GetComponent<Animator>();
        initialDelay -= spawnCooldown;
    }

    void FixedUpdate()
    {
        timer += Time.fixedDeltaTime * timeMultiplier;
        if(timer > spawnCooldown + initialDelay)
        {
            animator.SetTrigger("BubbleSpawn");
            initialDelay = 0;
            timer = 0;
        }
    }

    public void SpawnBubble()
    {
        int bubbleAmount = Random.Range(minAmountBubble, maxAmountBubble);
        StartCoroutine(BubbleBurst(bubbleAmount));
    }

    IEnumerator BubbleBurst(int _bubbleAmount)
    {
        float xPosition = Random.Range(-bubblePositionMaxDifference, bubblePositionMaxDifference);

        bubble = Instantiate(bubblePrefab, bubbleStorage);
        bubble.transform.position = (Vector2)transform.position + Vector2.right * xPosition + Vector2.up * spawnDistance;

        bubble.GetComponent<Bubble>().speed = speed;
        bubbleCount++;

        if(bubbleCount < _bubbleAmount)
        {

            float waitTime = Random.Range(minSmallBubbleCooldown, maxSmallBubbleCooldown);
            yield return new WaitForSeconds(waitTime);
            yield return StartCoroutine(BubbleBurst(_bubbleAmount));
        }
        else
        {
            bubbleCount = 0;
        }
    }

    public void SlowDownBubbleSpawner(float index)
    {
        timeMultiplier /= index;
        animator.speed = 1 / index;
    }
    public void ReverseSlowDownBubbleSpawner(float index)
    {
        timeMultiplier *= index;
        animator.speed = 1;
    }
}
