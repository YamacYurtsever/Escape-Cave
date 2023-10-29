using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BreathShake : MonoBehaviour
{
    //Breath Bar a bağlı

    //Slider fullken valuesu 10, 5 ten azsa fidget başla gibi, 
    public float fidgetAtPercentage = 50f;
    public float maxFidgetDistance = 10;
    public float fidgetSpeed = 0.7f;
    public float shakeAtPercentage = 30f;
    public float maxShakeDistance = 35f;
    public float shakeSpeed = 3.5f;

    private Slider breathSlider;
    private RectTransform breathTransform;
    private Vector2 originalPos;
    private bool chasingVector1 = false, chasingVector2 = false;
    private float fidgetAtValue, shakeAtValue;
    private Vector2 point1, point2;
    private Vector2 dir1, dir2;
    void Start()
    {
        breathTransform = GetComponent<RectTransform>();
        originalPos = breathTransform.anchoredPosition;
        breathSlider = GetComponent<Slider>();
        fidgetAtValue = breathSlider.maxValue * fidgetAtPercentage / 100;
        shakeAtValue = breathSlider.maxValue * shakeAtPercentage / 100;
    }

    void Update()
    {
        if (breathSlider.value < fidgetAtValue && breathSlider.value > shakeAtValue)
        {
            chasingVector2 = false;
            StartFidget();
        }
        else if (breathSlider.value < shakeAtValue)
        {
            chasingVector1 = false;
            StartShake();
        }
    }

    private void StartFidget()
    {
        if (!chasingVector1)
        {
            point1 = FindRandomFidgetPointWithinMaxDistance();
            Vector2 pos = breathTransform.anchoredPosition;
            dir1 = (point1 - pos).normalized;
        }
        ChaseVector(point1, fidgetSpeed, dir1);
    }

    private void StartShake()
    {
        if (!chasingVector2)
        {
            point2 = FindRandomShakePointWithinMaxDistance();
            Vector2 pos = breathTransform.anchoredPosition;
            dir2 = (point2 - pos).normalized;
        }
        ChaseVector(point2, shakeSpeed, dir2);
    }

    private void ChaseVector(Vector2 point, float speed, Vector2 dir)
    {
        breathTransform.anchoredPosition += dir * speed;
        
        if(Vector2.Distance(breathTransform.anchoredPosition, point) < 1)
        {
            chasingVector1 = false;
            chasingVector2 = false;
        }
    }

    private Vector2 FindRandomFidgetPointWithinMaxDistance()
    {
        float x = Random.Range(-1 * maxFidgetDistance, maxFidgetDistance);
        float y = Random.Range(-1 * maxFidgetDistance, maxFidgetDistance);

        Vector2 randomPoint = originalPos + new Vector2(x, y);
        return randomPoint;
    }

    private Vector2 FindRandomShakePointWithinMaxDistance()
    {
        float x = Random.Range(-1 * maxShakeDistance, maxShakeDistance);
        float y = Random.Range(-1 * maxShakeDistance, maxShakeDistance);

        Vector2 randomPoint = originalPos + new Vector2(x, y);
        return randomPoint;
    }
}
