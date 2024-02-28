using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class Asteroid : MonoBehaviour
{
    public new Rigidbody2D rigidbody { get; private set; }
    public SpriteRenderer spriteRenderer { get; private set; }
    public Sprite[] sprites;

    public float size = 1f;
    public float minSize = 0.35f;
    public float maxSize = 1.65f;
    public float movementSpeed = 50f;
    public float maxLifetime = 30f;
    public Player player;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        GameObject scoreGO = GameObject.Find("Player");
        player = scoreGO.GetComponent<Player>();
        //make each asteroid different
        spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];
        transform.eulerAngles = new Vector3(0f, 0f, Random.value * 360f);
        //make size and mass of the asteroid different based on their assigned random value above
        transform.localScale = Vector3.one * size;
        rigidbody.mass = size;
        //destroy when max life time is reached
        Destroy(gameObject, maxLifetime);

    }

    public void SetTrajectory(Vector2 direction)
    {
        //add force to asteroid
        rigidbody.AddForce(direction * movementSpeed);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Bullet"))
        {
            //check if asteroid is large enough to split in half
            if ((size * 0.5f) >= minSize)
            {
                CreateSplit();
                CreateSplit();
            }
            //destroy the current asteroid
            Destroy(this.gameObject);
            
            //Player[] players = FindObjectsOfType<Player>();
            //Player player = players.[0];
            //var objects = GameObject.FindGameObjectsWithTag("Player");
            if (size < 0.7f)
            {
                player.score += 25; // small asteroid
            }
            else if (size < 1.4f)
            {
                player.score += 10;// medium asteroid
            }
            else
            {
                player.score += 5;// large asteroid
            }
        }

    }   

    private Asteroid CreateSplit()
    {
        //set the new asteroid position but slightly different so they don't spawn on each other
        Vector2 position = transform.position;
        position += Random.insideUnitCircle * 0.5f;
        //create the new asteroid but half the sized
        Asteroid half = Instantiate(this, position, transform.rotation);
        half.size = size * 0.5f;
        half.SetTrajectory(Random.insideUnitCircle.normalized);
        return half;
    }
}

