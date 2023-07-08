using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMovementScript : MonoBehaviour
{

    private bool spaceWasPressed;
    private float horizontalInput;

    private float speed = 5f;
    private float jumpPover = 12f;
    private bool isFacingRight = true;
    private float timeForDeath = 0.8f;
    private bool isDead = false;
    private float exitAnimation = 0f;
    private int limitForMaxEnergy = 1250;
    private bool wasFalling = false;

    public Vector3 respawnPoint;
    private Animator playerAnimation;
    public GameObject afterDeathEmotion;
    public Text textForSoulEnergy;
    private AudioManager audioManager;

    [SerializeField] private Rigidbody2D rigidBodyComponent;
    [SerializeField] private Transform GroundChecker;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private bool isInvincible = false;
    [SerializeField] private ParticleSystem footPrintsJumping;
    [SerializeField] private ParticleSystem footPrintsMoveing;
    [SerializeField] private ParticleSystem afterDeathExplosion;

    // Start is called before the first frame update
    void Start()
    {
        PlayerStats.soulEnergyValue = PlayerStats.maxSoulEnergyValue;
        rigidBodyComponent = GetComponent<Rigidbody2D>();
        playerAnimation = GetComponent<Animator>();
        audioManager = FindObjectOfType<AudioManager>();
        textForSoulEnergy.text = "Soul Energy: " + PlayerStats.soulEnergyValue + " / " + PlayerStats.maxSoulEnergyValue;

        if (PlayerStats.hadGoneFrom1Location && SceneManager.GetActiveScene().buildIndex == 0)
        {
            respawnPoint = new Vector3(101, 5, 0); // Hardcoding is bad, this coordinates take place near the portal
            transform.position = respawnPoint;
        }
        else
        {
            respawnPoint = transform.position;
        }

        audioManager.ManageMusic();
        if (PlayerStats.hadGoneFrom1Location)
        {
            audioManager.Play("PortalSound");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            spaceWasPressed = true;
            SummonJumpParticles();
        }

        if (Input.GetKeyDown(KeyCode.I)) // cheats for testing
        {
            isInvincible = !isInvincible;
        }

        // logic for fall particles to appear
        if (rigidBodyComponent.velocity.y < -0.2f)
        {
            wasFalling = true;
        }
        if (wasFalling && IsGrounded())
        {
            wasFalling = false;
            SummonJumpParticles();
        }

        horizontalInput = Input.GetAxis("Horizontal");

        Flip();
        ParticleAirControll();
        SetSpawnPoint();

        playerAnimation.SetFloat("yVelocity", rigidBodyComponent.velocity.y);
        playerAnimation.SetFloat("SoulEnergy", PlayerStats.soulEnergyValue);
        playerAnimation.SetBool("IsOnGround", IsGrounded());

        textForSoulEnergy.text = "Soul Energy: " + PlayerStats.soulEnergyValue + " / " + PlayerStats.maxSoulEnergyValue;
    }

    // Upd for Phisicks
    void FixedUpdate()
    {
        if (!isDead)
        {
            rigidBodyComponent.velocity = new Vector2(horizontalInput * speed, rigidBodyComponent.velocity.y);
        }
        
        if (spaceWasPressed && !isDead)
        {
            rigidBodyComponent.velocity = new Vector2(rigidBodyComponent.velocity.x, jumpPover);
            spaceWasPressed = false;
        }

        PlayerDeathCheck();
        SoulEnergyRegen();
    }

    // prevents spawning particles in the air
    private void ParticleAirControll()
    {
        if (!IsGrounded() && footPrintsMoveing.isPlaying)
        {
            footPrintsMoveing.Stop();
        }
        else if (IsGrounded() && !footPrintsMoveing.isPlaying)
        {
            footPrintsMoveing.Play();
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(GroundChecker.position, 0.1f, groundLayer); 
    }

    private void Flip()
    {
        if (isFacingRight && horizontalInput < 0f)
        {
            isFacingRight = false;
            transform.Rotate(0f, 180f, 0f);
        }

        if (!isFacingRight && horizontalInput > 0f)
        {
            isFacingRight = true;
            transform.Rotate(0f, 180f, 0f);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "DeathZone")
        {
            PlayerStats.soulEnergyValue = 0;
        }
        else if (collision.tag == "Next Location")
        {
            PlayerStats.anger = false;
            audioManager.Play("PortalSound");
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                PlayerStats.hadGoneFrom1Location = true;
            }
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            respawnPoint = transform.position;
        }
        else if (collision.tag == "Previous Location")
        {
            PlayerStats.anger = false;
            audioManager.Play("PortalSound");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
            respawnPoint = transform.position;
        }
        else if (collision.tag == "EnergyEmision")
        {
            if (PlayerStats.maxSoulEnergyValue < limitForMaxEnergy) // no more then 1250
            {
                PlayerStats.maxSoulEnergyValue += 25; // one death heal
            }
            audioManager.Play("PickUp");
            collision.gameObject.SetActive(false);
        }
    }

    private void PlayerDeathCheck()
    {
        if (PlayerStats.soulEnergyValue <= 0 && !isDead) // making timer + entering death animation
        {
            audioManager.Play("MGDeath");
            rigidBodyComponent.gravityScale = 0f;
            rigidBodyComponent.velocity = new Vector2(0, 0);
            isDead = true;
            wasFalling = false;
            exitAnimation = Time.time + timeForDeath;
            Instantiate(afterDeathExplosion, transform.position, transform.rotation);
        }
        else if (isDead && Time.time >= exitAnimation)
        {
            if (PlayerStats.maxSoulEnergyValue <= 0) // maxenergy <= 0 ---- player is truly dead
            {
                gameObject.SetActive(false);
            }
            else
            {
                Instantiate(afterDeathEmotion, transform.position, transform.rotation);
                transform.position = respawnPoint;
                rigidBodyComponent.gravityScale = 2f; // hardcoded gravity
                isDead = false;
                PlayerStats.soulEnergyValue = 1;
                if (PlayerStats.maxSoulEnergyValue > 0)
                {
                    PlayerStats.maxSoulEnergyValue -= 25; // cann't be negative
                }
            }
        }
    }

    private void SoulEnergyRegen()
    {
        if (PlayerStats.soulEnergyValue < PlayerStats.maxSoulEnergyValue && !isDead)
        {
            PlayerStats.soulEnergyValue++;
        }
    }

    // for Particles bursting when jumping
    private void SummonJumpParticles()
    {
        if (!footPrintsJumping.isPlaying)
        {
            footPrintsJumping.Play();
        }
        else
        {
            footPrintsJumping.Stop();
        }
    }

    private void SetSpawnPoint()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (PlayerStats.soulEnergyValue > 500)
            {
                respawnPoint = transform.position;
                PlayerStats.soulEnergyValue -= 500;
            }
            else
            {
                PlayerStats.soulEnergyValue = 0;
            }
        }
    }

    public void TakeDamage (int damage)
    {
        if (!isInvincible)
        {
            if (PlayerStats.soulEnergyValue >= damage)
            {
                PlayerStats.soulEnergyValue -= damage;
            }
            else
            {
                PlayerStats.soulEnergyValue = 0;
            }
        }
    }
}
