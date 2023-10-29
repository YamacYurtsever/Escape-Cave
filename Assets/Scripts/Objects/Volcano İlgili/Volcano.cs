using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Volcano : MonoBehaviour
{
    public GameObject lavaBallPrefab;
    public Transform lavaBallStorage;
    public GameObject groundLavaPrefab;
    public Transform groundLavaStorage;
    public LayerMask groundLayer;
    public float spewLavaForce = 10f;
    public float lavaBallGravity = 1f;
    public float spawnDistance = 1f;
    public float cooldown = 3.5f;
    public float initialDelay= 0.5f;
    public AudioSource volcanoErupt;

    private GameObject player;
    private GameObject lavaBall;
    private LavaBall lavaBallScript;
    private Transform lavaBallTransform;
    private Rigidbody2D lavaBallRb;
    private Animator animator;
    private float timer;
    private float timeMultiplier = 1f;
    private float eulerAngleZ;
    private Vector2 throwDirection;
    private float gravityMultiplier = 1;

    void Start()
    {
        animator = GetComponent<Animator>();
       
        eulerAngleZ = transform.rotation.eulerAngles.z;
        throwDirection = new Vector2( Mathf.Cos(Mathf.Deg2Rad * (eulerAngleZ + 90)) , Mathf.Sin(Mathf.Deg2Rad * (eulerAngleZ + 90)) );
        throwDirection = throwDirection.normalized;
        initialDelay -= cooldown;
    }

    void FixedUpdate()
    {
        timer += Time.fixedDeltaTime * timeMultiplier;
        if (timer > cooldown + initialDelay)
        {
            //if so, starts object animation
            animator.SetTrigger("Spew");
            initialDelay = 0;
            timer = 0;
        }
    }

    public void SpewLava()
    {
        lavaBall = Instantiate(lavaBallPrefab, lavaBallStorage);
        lavaBallTransform = lavaBall.transform;
        lavaBallRb = lavaBall.GetComponent<Rigidbody2D>();
        lavaBallScript = lavaBall.GetComponent<LavaBall>();

        volcanoErupt.Play();

        lavaBallRb.gravityScale = lavaBallGravity * gravityMultiplier * gravityMultiplier;
        lavaBallScript.SetValues(groundLayer.value, groundLavaPrefab, groundLavaStorage);
        lavaBallTransform.position = (Vector2) transform.position + throwDirection * spawnDistance;
        lavaBallRb.AddForce(throwDirection * spewLavaForce * gravityMultiplier, ForceMode2D.Impulse);
    }

    public void SlowDownVolcano(float index)
    {
        timeMultiplier /= index;
        animator.speed = 1 / index;
        gravityMultiplier = 1 / index;
    }
    public void ReverseSlowDownVolcano(float index)
    {
        timeMultiplier *= index;
        animator.speed = 1;
        gravityMultiplier = 1;
    }
}