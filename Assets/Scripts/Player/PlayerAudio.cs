using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    public float minWaitWalk = 0.05f, maxWaitWalk = 0.12f;
    public AudioSource step1, step2, step3;
    public AudioSource jump;
    public AudioSource land;
    public AudioSource death;
    public AudioSource splashAudio;
    public AudioSource groundMusic, underwaterMusic;
    public AudioSource lighterOn, lighterOff;
    public float changeTime;
    
    private float maxGround, maxWater;
    private float repeatTime = 0.02f;
    private float speed1, speed2;
    private float leftOff1, leftOff2;

    private void Start()
    {
        maxGround = groundMusic.volume;
        maxWater = underwaterMusic.volume;
        speed1 = maxGround / (changeTime / repeatTime);
        speed2 = maxWater / (changeTime / repeatTime);
    }

    public void WalkAnimationTrigger()
    {
        float waitTime = Random.Range(minWaitWalk, maxWaitWalk);
        StartCoroutine(WaitAndPlaySound(waitTime));
    }
    IEnumerator WaitAndPlaySound(float time)
    {
        yield return new WaitForSeconds(time);

        int chooseStep = Random.Range(1, 4);

        if (chooseStep == 1) step1.Play();
        if (chooseStep == 2) step2.Play();
        if (chooseStep == 3) step3.Play();
    }

    public void JumpSound()
    {
        jump.Play();
    }

    public void LandSound()
    {
        land.Play();
    }

    public void PlayerDiesSound()
    {
        death.Play();
    }

    public void WaitAndTurnOffLighterSound()
    {
        StartCoroutine(zzz());
    }

    IEnumerator zzz()
    {
        yield return new WaitForSeconds(1.1f);
        lighterOff.Stop();
    }

    public void SwitchtoUnderWaterMusic()
    {
        underwaterMusic.volume = 0;
        underwaterMusic.Play();
        underwaterMusic.time = leftOff1;

        StartCoroutine(SlowlyChangetoUnderwater());
    }

    IEnumerator SlowlyChangetoUnderwater()
    {
        underwaterMusic.volume += speed2;
        if(groundMusic.volume >= speed1) groundMusic.volume -= speed1;

        if (underwaterMusic.volume < maxWater)
        {
            yield return new WaitForSeconds(repeatTime);
            yield return SlowlyChangetoUnderwater();
        }
        else
        {
            leftOff2 = groundMusic.time;
            groundMusic.Stop();
        }
    }

    public void SwitchbacktoGroundMusic()
    {
        groundMusic.volume = 0;
        groundMusic.Play();
        groundMusic.time = leftOff2;

        StartCoroutine(SlowlyChangetoGround());
    }

    IEnumerator SlowlyChangetoGround()
    {
        groundMusic.volume += speed1;
        if (underwaterMusic.volume >= speed2) underwaterMusic.volume -= speed2;

        if(groundMusic.volume < maxGround)
        {
            yield return new WaitForSeconds(repeatTime);
            yield return SlowlyChangetoGround();
        }
        else
        {
            leftOff1 = underwaterMusic.time;
            underwaterMusic.Stop();
        }
    }
}
