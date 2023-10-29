using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaBall : MonoBehaviour
{
    private GameObject groundLavaPrefab;
    private Transform groundLavaStorage;
    private PlayerDeath playerDeath;
    private GameObject player;
    private Collider2D playerCol;
    private Collider2D lavaBallCol;
    private Rigidbody2D rb;
    private int groundLayerValue;

    // Start is called before the first frame update
    void Start()
    {
        lavaBallCol = GetComponent<Collider2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerCol = player.GetComponent<Collider2D>();
        playerDeath = GameObject.FindGameObjectWithTag("PlayerDeath").GetComponent<PlayerDeath>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetValues(int _layerValue, GameObject _groundLava, Transform _groundLavaStorage)
    {
        groundLayerValue = _layerValue;
        groundLavaPrefab = _groundLava;
        groundLavaStorage = _groundLavaStorage;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckCollisionWithPlayer();
        CheckCollisionWithGround();
    }

    private void CheckCollisionWithPlayer()
    {
        if(lavaBallCol.IsTouching(playerCol))
        {
            playerDeath.Death();
            Destroy(gameObject);
        }
    }
    private void CheckCollisionWithGround()
    {
        if (lavaBallCol.IsTouchingLayers(groundLayerValue))
        {
            //yere değince başka bir şey yapcaksa buraya.
            SpawnGroundLava();
            Destroy(gameObject);
        }
    }
    private void SpawnGroundLava()
    {
        GameObject groundLava = Instantiate(groundLavaPrefab, groundLavaStorage);
        groundLava.transform.position = (Vector2) transform.position + new Vector2(0, 0.1f);
    }

    public void SlowDownLavaBall(float index)
    {
        rb.velocity /= index;
        rb.gravityScale /= (index * index);
    }
    public void ReverseSlowDownLavaBall(float index)
    {
        rb.velocity *= index;
        rb.gravityScale *= (index * index);
    }
}
