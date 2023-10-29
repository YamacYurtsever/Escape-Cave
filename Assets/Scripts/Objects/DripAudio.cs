using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DripAudio : MonoBehaviour
{
    public AudioSource drip1, drip2, drip3;
    public void PlayDripNoise()
    {
        int chooseDrip = Random.Range(1, 4);

        if (chooseDrip == 1) drip1.Play();
        if (chooseDrip == 2) drip2.Play();
        if (chooseDrip == 3) drip3.Play();
    }
}
