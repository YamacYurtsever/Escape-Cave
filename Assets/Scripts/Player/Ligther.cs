using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ligther : MonoBehaviour
{
    public GameObject lighter;
    public Vector2 offsetVector;
    public Vector2 offsetVector2;
    public float positionMultiplier;
    public GameObject circleLightSource;
    public GameObject spotLightSource;
    public Animator lighterAnimator;
    public bool isStunned = false;
    public float newLocationX1, newLocationX2;

    private Camera cam;
    private float playerScaleX, playerScaleY;
    private Transform lighterTransform;
    private SwimForce swimForce;
    private float newRotationZ;
    private int multiplier;
    private PlayerController playerController;

    void Awake()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        lighterTransform = lighter.transform;
        playerScaleX = transform.localScale.x;
        playerScaleY = transform.localScale.y;
        swimForce = GetComponent<SwimForce>();
        playerController = GetComponent<PlayerController>();
    }


    void Update()
    {
        Vector2 mouseDir = GetDirectionofMouse();

        if (!playerController.isWallSliding)
        {
            RotationOfLighter(mouseDir);
            PositionLighterToMouse(mouseDir);
        }
        else
        {
            RotationOfLighter(new Vector2 (transform.localScale.x, 0));
            PositionLighterToMouse(new Vector2(transform.localScale.x, 0));
        }

        Illuminate();

        if (swimForce.touchingWater)
        {
            //circleLightSource.SetActive(false);
            lighter.SetActive(false);
            spotLightSource.SetActive(false);
            lighterAnimator.SetBool("Open", false);
        }
        else
        {
            //circleLightSource.SetActive(true);
            lighter.SetActive(true);
            spotLightSource.SetActive(true);
            lighterAnimator.SetBool("Open", true);
        }
    }

    private Vector2 GetDirectionofMouse()
    {
        Vector2 direction;
        Vector2 mousePosInWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 pos = (Vector2) transform.position + offsetVector;
        direction = mousePosInWorld - pos;

        return direction;
    }

    private void RotationOfLighter(Vector2 dir)
    {
        if (isStunned)
        {
            if (dir.x < 0)
            {
                transform.position = new Vector2(newLocationX1, transform.position.y);
                transform.localScale = new Vector2(-1 * playerScaleX, playerScaleY);
                transform.eulerAngles = new Vector3(0, 0, -90);
            }
            else
            {
                transform.position = new Vector2(newLocationX2, transform.position.y);
                transform.localScale = new Vector2(playerScaleX, playerScaleY);
                transform.eulerAngles = new Vector3(0, 0, 90);
            }
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            if (dir.x < 0)
            {
                multiplier = -1;
                if (!playerController.isWallSliding)
                    transform.localScale = new Vector2(-1 * playerScaleX, playerScaleY);
            }
            else
            {
                multiplier = 1;
                if (!playerController.isWallSliding)
                    transform.localScale = new Vector2(playerScaleX, playerScaleY);
            }
        }

        float angle = Vector2.Angle(dir, Vector2.up);
        newRotationZ = multiplier * (90 - angle);
        lighterTransform.eulerAngles = new Vector3(0, 0, newRotationZ);
    }

    private void PositionLighterToMouse(Vector2 dir)
    {
        float angleRad = Mathf.Atan2(dir.y, dir.x);
        if (dir.x > 0) angleRad += Mathf.Deg2Rad;
        if (dir.x < 0) angleRad -= Mathf.Deg2Rad;
        Vector2 newVector = new Vector2(dir.magnitude * Mathf.Cos(angleRad), dir.magnitude * Mathf.Sin(angleRad));

        lighterTransform.position = (Vector2)transform.position + offsetVector2 + newVector.normalized * positionMultiplier;
    }
    
    private void Illuminate()
    {
        circleLightSource.transform.position = new Vector2(transform.position.x, transform.position.y - 0.5f);
        spotLightSource.transform.position = (Vector2)transform.position + new Vector2(lighter.transform.localPosition.x * multiplier, lighter.transform.localPosition.y);

        //rotation of spotlight
        spotLightSource.transform.eulerAngles = new Vector3(0, 0, newRotationZ - multiplier * 90);
    }
}