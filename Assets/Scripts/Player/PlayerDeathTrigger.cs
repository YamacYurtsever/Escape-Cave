using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathTrigger : MonoBehaviour
{
    private PlayerDeath playerDeath;

    private void Start()
    {
        playerDeath = GameObject.FindGameObjectWithTag("PlayerDeath").GetComponent<PlayerDeath>();
    }

    public void PlayerDies()
    {
        playerDeath.PlayerSpriteDisappear();
    }
}
