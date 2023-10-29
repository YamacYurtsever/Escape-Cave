using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDrop : MonoBehaviour
{
    public float fallSpeed = 10f;
    public GameObject originalDrop;
    public Transform dropStorage;
    public float dropCooldown;
    public float initialDelay = 0f;

    private Animator dropperAnimator;
    private GameObject drop;
    private Vector2 dropperPos;
    private float timeMultiplier = 1;
    private float timer = 0;

    void Start()
    {
        dropperPos = transform.position;
        dropperAnimator = GetComponent<Animator>();
        initialDelay -= dropCooldown;
    }

    void FixedUpdate()
    {
        timer += Time.fixedDeltaTime * timeMultiplier;
        if(timer > dropCooldown + initialDelay)
        {
            //if so, starts object animation
            dropperAnimator.SetTrigger("Drop");
            initialDelay = 0;
            timer = 0;
        }
    }

    public void StartDropFall()
    {
        //Make a drop and set Rigidbody and Collider
        drop = Instantiate(originalDrop, dropStorage);
        drop.transform.position = dropperPos;
        drop.GetComponent<DropDestroy>().fallSpeed = fallSpeed;

        //Dropper goes back from animation to normal
        dropperAnimator.SetTrigger("Back");
    }

    public void SlowDownFall(float index)
    {
        timeMultiplier /= index;
        dropperAnimator.speed = 1 / index;
    }
    public void ReverseSlowDownFall(float index)
    {
        timeMultiplier *= index;
        dropperAnimator.speed = 1;
    }
}
