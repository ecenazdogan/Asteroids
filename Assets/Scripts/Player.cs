using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    public new Rigidbody2D rigidbody { get; private set; }
    public Bullet bulletPrefab;

    public float thrustSpeed = 1f;
    public bool thrusting { get; private set; }

    public float turnDirection { get; private set; } = 0f;
    public float rotationSpeed = 0.1f;

    public float respawnDelay = 2f;
    public float respawnInvulnerability = 3f;

    public ParticleSystem explosionEffect;

    public int score=0;
    static private int highscore = 0;
    public int defaultlives = 3;
    public int livesCounter;
    public TMP_Text livesText;
    public TMP_Text scoreText;
    public TMP_Text highscoreText;


    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        thrusting = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)){
            turnDirection = 1f;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)){
            turnDirection = -1f;
        }
        else{
            turnDirection = 0f;
        }
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)){
            Shoot();
        }
        livesText.text = "x " + livesCounter;
        if (livesCounter < 1)
        {
            GameOver();
        }
        scoreText.text = "Score: " + score;
        highscoreText.text = "High Score: " + highscore;
        if (score > highscore)
        {
            highscore = score;
        }
    }
    private void FixedUpdate()
    {
        if (thrusting)
        {
            rigidbody.AddForce(this.transform.up * this.thrustSpeed);
        }

        if (turnDirection != 0f)
        {
            rigidbody.AddTorque(this.rotationSpeed * turnDirection);
        }
    }
    private void Shoot()
    {
        Bullet bullet = Instantiate(this.bulletPrefab, transform.position, transform.rotation);
        bullet.Project(transform.up);
    }
    private void TurnOnCollisions()
    {
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Asteroid")){
            TakeLife();
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = 0f;
            gameObject.SetActive(false);
            PlayerDeath();
        }
    }
    public void PlayerDeath()
    {
        explosionEffect.transform.position = transform.position;
        explosionEffect.Play();
        this.gameObject.SetActive(false);
        Invoke(nameof(Respawn), respawnDelay);
    }
    public void AsteroidDestroyed(Asteroid asteroid)
    {
        explosionEffect.transform.position = asteroid.transform.position;
        explosionEffect.Play();
    }
    private void Start()
    {
        NewGame();
        score = 0;
        defaultlives = 3;
        livesCounter = defaultlives;
    }
    public void NewGame()
    {
        Asteroid[] asteroids = FindObjectsOfType<Asteroid>();
        for (int i = 0; i < asteroids.Length; i++)
        {
            Destroy(asteroids[i].gameObject);
        }
        Respawn();
    }
    public void Respawn()
    {
        this.transform.position = Vector3.zero;
        this.gameObject.layer = LayerMask.NameToLayer("Ignore Collisions");
        this.gameObject.SetActive(true);
        Invoke(nameof(TurnOnCollisions), respawnInvulnerability);
    }
    public void TakeLife()
    {
        livesCounter--;
    }
    public void GameOver()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void HighScore() 
    {
        if (score > highscore)
        {
            highscore = score;
        }
    }

}
