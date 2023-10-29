using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    public PlayerDeath playerDeath;
    
    private Collider2D spikeCol, playerCol;

    private void Start()
    {
        spikeCol = GetComponent<Collider2D>();
        playerCol = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (spikeCol.IsTouching(playerCol))
        {
            playerDeath.Death();
        }
    }
}
