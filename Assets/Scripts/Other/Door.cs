using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public float detectionRangeX;
    public float detectionRangeY;
    public LayerMask playerLayer;
    public GameObject player;
    public AudioSource doorAudio;

    private Collider2D playerCol;
    private bool doorIsOpen = false;
    private Collider2D col;
    private Animator animator;
    private bool playerInRange = false;
    private bool onlyOnce = true;

    void Start()
    {
        col = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        playerCol = player.GetComponent<Collider2D>();
    }

    void Update()
    {
        playerInRange = Physics2D.OverlapArea(new Vector2(transform.position.x + detectionRangeX, transform.position.y + detectionRangeY), 
                                              new Vector2(transform.position.x - detectionRangeX, transform.position.y - detectionRangeY),
                                              playerLayer);
        if (playerInRange)
        {
            animator.SetBool("isOpen", true);
            if(onlyOnce)
            {
                doorAudio.Play();
                onlyOnce = false;
            }
            doorIsOpen = true;
            if (col.IsTouching(playerCol) && doorIsOpen)
            {
                SceneLoader.LoadNextScene();
            }
        }
    }
}