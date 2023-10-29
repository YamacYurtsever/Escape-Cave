using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering.Universal;

public class PlayerDeath : MonoBehaviour
{
    public float lightInitialAlpha;
    public float lightFinalAlpha;
    public float delayAfterAnimationBeforeVanish;
    public float delayBeforeRespawn;
    public float speed = 0.4f;
    public CinemachineVirtualCamera cinemachineVCam;
    public CinemachineBrain cinemachineBrain;
    public Transform playerMadeLightsStorage;
    public GameObject deathLight;
    public Light2D deathLightScript;
    public SceneLoader sceneLoader;
    public GameObject breathBar;
    public bool isDead = false;

    private GameObject player;
    private SpriteRenderer lighterSprite;
    private Collider2D col;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer sr;
    private Ligther lighter;
    private PlayerMovement movement;
    private PlayerController controller;
    private SwimForce swimForce;
    private BreathingSystem breathSystem;
    private float deathColorAlpha;
    private Color deathLightColor;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        lighterSprite = player.transform.GetChild(0).GetComponent<SpriteRenderer>();
        col = player.GetComponent<Collider2D>();
        rb = player.GetComponent<Rigidbody2D>();
        sr = player.GetComponent<SpriteRenderer>();
        lighter = player.GetComponent<Ligther>();
        movement = player.GetComponent<PlayerMovement>();
        swimForce = player.GetComponent<SwimForce>();
        controller = player.GetComponent<PlayerController>();
        animator = player.GetComponent<Animator>();
        breathSystem = player.GetComponent<BreathingSystem>();
        deathColorAlpha = lightInitialAlpha;
    }

    public void Death()
    {
        isDead = true;
        col.enabled = false;
        rb.bodyType = RigidbodyType2D.Static;
        lighter.enabled = false;
        movement.enabled = false;
        controller.enabled = false;
        swimForce.enabled = false;
        lighterSprite.enabled = false;
        breathSystem.enabled = false;
        breathBar.SetActive(false);

        if(lighter.isStunned)
        {
            player.transform.eulerAngles = new Vector3(0, 0, 0);
        }

        int childcount = playerMadeLightsStorage.childCount;
        for(int i = 0; i < childcount; i++)
        {
            playerMadeLightsStorage.GetChild(i).gameObject.SetActive(false);
        }
        deathLight.transform.position = player.transform.position;
        deathLight.SetActive(true);

        deathLightColor = deathLightScript.color;
        deathLightColor = new Color(deathLightColor.r, deathLightColor.g, deathLightColor.b, lightInitialAlpha/255);
        deathLightScript.color = deathLightColor;
        animator.SetBool("IsDead", true);
        StartCoroutine(SlowlyIncreaseLight());
    }

    IEnumerator SlowlyIncreaseLight()
    {
        float increaseAmount = (250 - lightInitialAlpha) / (delayAfterAnimationBeforeVanish/0.02f);
        deathColorAlpha += increaseAmount;
        deathLightColor = new Color(deathLightColor.r, deathLightColor.g, deathLightColor.b, deathColorAlpha/255);
        deathLightScript.color = deathLightColor;

        if (deathColorAlpha < 250)
        {
            yield return new WaitForSeconds(0.02f);
            yield return StartCoroutine(SlowlyIncreaseLight());
        }
    }

    IEnumerator SlowlyDecreaseLight()
    {
        float decreaseAmount = (250 - lightFinalAlpha) / (delayBeforeRespawn / 0.02f);
        deathColorAlpha -= decreaseAmount;
        deathLightColor = new Color(deathLightColor.r, deathLightColor.g, deathLightColor.b, deathColorAlpha/255);
        deathLightScript.color = deathLightColor;

        if (deathColorAlpha > lightFinalAlpha)
        {
            yield return new WaitForSeconds(0.02f);
            yield return StartCoroutine(SlowlyDecreaseLight());
        }
    }

    public void PlayerSpriteDisappear()
    {
        StartCoroutine(WaitAndPlayerSpriteDisappear());
    }

    IEnumerator WaitAndPlayerSpriteDisappear()
    {
        yield return new WaitForSeconds(delayAfterAnimationBeforeVanish);
        sr.enabled = false;
        cinemachineBrain.enabled = false;
        cinemachineVCam.Follow = null;
        yield return StartCoroutine(WaitAndRespawnPlayer());
    }

    IEnumerator WaitAndRespawnPlayer()
    {
        StartCoroutine(SlowlyDecreaseLight());
        yield return new WaitForSeconds(delayBeforeRespawn);

        RestartLevel();
    }

    private void RestartLevel()
    {
        sceneLoader.RestartScene();
    }
}
