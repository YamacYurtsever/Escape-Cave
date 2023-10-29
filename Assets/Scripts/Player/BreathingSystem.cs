using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BreathingSystem : MonoBehaviour
{
    public int breathHoldSeconds = 5;
    public float breathDecrease = 0.1f;
    public float breathIncrease = 0.1f;
    public bool stillHolding = false;
    public GameObject breathBar;
    public bool wasInWater = false;

    private Slider slider;
    private PlayerDeath playerDeath;
    private SwimForce swimForce;

    [Header("Time Slowing Containers")]
    public float slowDownIndex = 3;
    public GameObject droppers;
    public GameObject drops;
    public GameObject stalactites;
    public GameObject snails;
    public GameObject fishsPatrol;
    public GameObject fishsFollow;
    public GameObject volcanoes;
    public GameObject lavaBalls;
    public GameObject groundLavas;
    public GameObject bubbleSpawners;
    public GameObject bubbles;
    public GameObject jellyfishes;

    private void Start()
    {
        playerDeath = GameObject.FindGameObjectWithTag("PlayerDeath").GetComponent<PlayerDeath>();
        slider = breathBar.GetComponent<Slider>();
        swimForce = GetComponent<SwimForce>();
        SetMaxBreath(breathHoldSeconds);
    }

    public void SetMaxBreath(int breathSeconds)
    {
        slider.maxValue = breathSeconds;
        slider.value = breathSeconds;
    }

    public void StartHoldingBreath()
    {
        breathBar.SetActive(true);
        stillHolding = true;
        if (!swimForce.inWater)
            SlowDownTime();
        StartCoroutine(StartHoldingBreathTimer());
        breathBar.transform.GetChild(0).GetComponent<Image>().color = Color.red; //Color Change
    }

    public void SlowDownTime()
    {
        foreach (Transform dropper in droppers.transform)
        {
            dropper.GetComponent<WaterDrop>().SlowDownFall(slowDownIndex);
        }
        foreach (Transform drop in drops.transform)
        {
            drop.GetComponent<DropDestroy>().SlowDownFall(slowDownIndex);
        }
        foreach (Transform stalactite in stalactites.transform)
        {
            stalactite.GetComponent<ObjectFall>().SlowDownFall(slowDownIndex);
        }
        foreach (Transform snail in snails.transform)
        {
            snail.GetComponent<Snail>().SlowDownSnail(slowDownIndex);
        }
        foreach (Transform fish in fishsPatrol.transform)
        {
            fish.GetComponent<FishPatrol>().SlowDownFish(slowDownIndex);
        }
        foreach (Transform fish in fishsFollow.transform)
        {
            fish.GetComponent<FishFollow>().SlowDownFish(slowDownIndex);
        }
        foreach (Transform volcano in volcanoes.transform)
        {
            volcano.GetComponent<Volcano>().SlowDownVolcano(slowDownIndex);
        }
        foreach (Transform lavaBall in lavaBalls.transform)
        {
            lavaBall.GetComponent<LavaBall>().SlowDownLavaBall(slowDownIndex);
        }
        foreach (Transform groundLava in groundLavas.transform)
        {
            groundLava.GetComponent<GroundLava>().SlowDown(slowDownIndex);
        }
        foreach (Transform bubble in bubbles.transform)
        {
            bubble.GetComponent<Bubble>().SlowDownBubble(slowDownIndex);
        }
        foreach (Transform bubbleSpawner in bubbleSpawners.transform)
        {
            bubbleSpawner.GetComponent<BubbleSpawn>().SlowDownBubbleSpawner(slowDownIndex);
        }
        foreach (Transform jellyfish in jellyfishes.transform)
        {
            jellyfish.GetComponent<JellyFish>().SlowDownJellyfish(slowDownIndex);
        }
    }

    private IEnumerator StartHoldingBreathTimer()
    {
        slider.value -= breathDecrease;
        yield return new WaitForSeconds(0.1f);
        if (slider.value <= 0)
        {
            if (swimForce.submerged)
                playerDeath.Death();
            else
            {
                stillHolding = false;
                StartBreathing();
            }
        }
        else if (stillHolding == true)
        {
            StartCoroutine(StartHoldingBreathTimer());
        }
    }

    public void StartBreathing()
    {
        stillHolding = false;
        breathBar.transform.GetChild(0).GetComponent<Image>().color = Color.white; //Color Change
        if (!wasInWater)
        {
            wasInWater = false;
            ReverseSlowDownTime();
        }
        StartCoroutine(RegainBreath());
    }

    public void ReverseSlowDownTime()
    {
        foreach (Transform dropper in droppers.transform)
        {
            dropper.GetComponent<WaterDrop>().ReverseSlowDownFall(slowDownIndex);
        }
        //there is an issue here as new drops can be created while holding breath and those aren't affected, put the fall speed to dropper
        foreach (Transform drop in drops.transform) 
        {
            drop.GetComponent<DropDestroy>().ReverseSlowDownFall(slowDownIndex);
        }
        foreach (Transform stalactite in stalactites.transform)
        {
            stalactite.GetComponent<ObjectFall>().ReverseSlowDownFall(slowDownIndex);
        }
        foreach (Transform snail in snails.transform)
        {
            snail.GetComponent<Snail>().ReverseSlowDownSnail(slowDownIndex);
        }
        foreach (Transform fish in fishsPatrol.transform)
        {
            fish.GetComponent<FishPatrol>().ReverseSlowDownFish(slowDownIndex);
        }
        foreach (Transform fish in fishsFollow.transform)
        {
            fish.GetComponent<FishFollow>().ReverseSlowDownFish(slowDownIndex);
        }
        foreach (Transform volcano in volcanoes.transform)
        {
            volcano.GetComponent<Volcano>().ReverseSlowDownVolcano(slowDownIndex);
        }
        foreach (Transform lavaBall in lavaBalls.transform)
        {
            lavaBall.GetComponent<LavaBall>().ReverseSlowDownLavaBall(slowDownIndex);
        }
        foreach (Transform groundLava in groundLavas.transform)
        {
            groundLava.GetComponent<GroundLava>().ReverseSlowDown(slowDownIndex);
        }
        foreach (Transform bubble in bubbles.transform)
        {
            bubble.GetComponent<Bubble>().ReverseSlowDownBubble(slowDownIndex);
        }
        foreach (Transform bubbleSpawner in bubbleSpawners.transform)
        {
            bubbleSpawner.GetComponent<BubbleSpawn>().ReverseSlowDownBubbleSpawner(slowDownIndex);
        }
        foreach (Transform jellyfish in jellyfishes.transform)
        {
            jellyfish.GetComponent<JellyFish>().ReverseSlowDownJellyfish(slowDownIndex);
        }
    }

    private IEnumerator RegainBreath()
    {
        if (slider.value < slider.maxValue)
        {
            slider.value += breathIncrease;
            yield return new WaitForSeconds(0.1f);
            if (!stillHolding)
                StartCoroutine(RegainBreath());
            else
            {
                breathBar.transform.GetChild(0).GetComponent<Image>().color = Color.red; //Color Change
            }
        }
        else
        {
            breathBar.transform.GetChild(0).GetComponent<Image>().color = Color.red; //Color Change
            breathBar.SetActive(false);
        }
    }

    public void IncreaseBreath(float increase)
    {
        if(slider.value < slider.maxValue)
        {
            slider.value += increase;
        }
    }
}
