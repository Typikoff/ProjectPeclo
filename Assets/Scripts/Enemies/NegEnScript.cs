using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NegEnScript : MonoBehaviour
{

    public Rigidbody2D rigidBodyComponent;
    public Transform player;
    private AudioManager audioManager;
    private float reactDistance = 12f;
    private float xSpeed = 3f;
    private float ySpeed = 1.5f;
    public bool playerIsNear = false;

    private int maxSoulEnergyValue = 150;
    public int soulEnergyValue = 90;

    public int damageValue = 200;
    private int AtackConsumation = 100;
    private float atackCooldown = 1f;
    private float timeOfNextAtack = 0f;
    public bool hadAtacked = false;
    private bool timerSettedUp = false;

    private float timeForNextMovement = 0f;
    private float timeBetweenMoves = 0.2f;
    private bool canMove = true;
    private bool timerToReset = true;
    private bool isFreezed = false;

    private Animator NegativeEmotionAnimator;
    [SerializeField] private ParticleSystem deathExplosionParticles;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").transform;
        rigidBodyComponent = GetComponent<Rigidbody2D>();
        NegativeEmotionAnimator = GetComponent<Animator>();
        audioManager = FindObjectOfType<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        NegativeEmotionAnimator.SetFloat("SoulEnergy", soulEnergyValue);
        if (soulEnergyValue > maxSoulEnergyValue)
        {
            NegativeEmotionAnimator.SetBool("IsOverCharged", true);
        }
        else
        {
            NegativeEmotionAnimator.SetBool("IsOverCharged", false);
        }
    }

    void FixedUpdate()
    {
        IsDead();
        if (!isFreezed)
        {
            SoulEnergyRegen();
            WaitForNextMovement();
            if (canMove)
            {
                MovementLogic();
            }
        }
    }

    void MovementLogic()
    {
        canMove = false;
        Movement();
        if (hadAtacked)
        {
            if (!timerSettedUp)
            {
                audioManager.Play("NegEmHit"); // here it plays right after hit
                timeOfNextAtack = Time.time + atackCooldown;
                timerSettedUp = true;
            }
            rigidBodyComponent.velocity = new Vector2(rigidBodyComponent.velocity.x * -1, rigidBodyComponent.velocity.y * -1);
            if (Time.time >= timeOfNextAtack)
            {
                hadAtacked = false;
                timerSettedUp = false;
            }
        }
    }

    void Movement()
    {
        playerIsNear = false;
        rigidBodyComponent.velocity = new Vector2(0, 0);

        if (player.position.x < transform.position.x && (transform.position.x - player.position.x <= reactDistance)) // player is left
        {
            playerIsNear = true;
            rigidBodyComponent.velocity = new Vector2(-1 * xSpeed, rigidBodyComponent.velocity.y);
        }
        else if (player.position.x > transform.position.x && (player.position.x - transform.position.x <= reactDistance)) // player is right
        {
            playerIsNear = true;
            rigidBodyComponent.velocity = new Vector2(xSpeed, rigidBodyComponent.velocity.y);
        }

        if (playerIsNear)
        {
            if (player.position.y < transform.position.y && (transform.position.y - player.position.y <= reactDistance)) // player is down
            {
                rigidBodyComponent.velocity = new Vector2(rigidBodyComponent.velocity.x, -1 * ySpeed);
            }
            if (player.position.y > transform.position.y && (player.position.y - transform.position.y <= reactDistance)) // player is up
            {
                rigidBodyComponent.velocity = new Vector2(rigidBodyComponent.velocity.x, ySpeed);
            }
        }
    }

    private void WaitForNextMovement() // prevents moving too rapidly
    {
        if (!canMove && timerToReset)
        {
            timeForNextMovement = Time.time + timeBetweenMoves;
            timerToReset = false;
        }

        if (!canMove && Time.time >= timeForNextMovement)
        {
            canMove = true;
            timerToReset = true;
        }


    }

    private void IsDead()
    {
        if (soulEnergyValue < 1)
        {
            if (!isFreezed)
            {
                Instantiate(deathExplosionParticles, transform.position, transform.rotation);
                audioManager.Play("NegEmDeath");
                Destroy(gameObject, 3.3f);
                isFreezed = true;
            }
            rigidBodyComponent.velocity = new Vector2(0, 0);
        }
    }

    private void SoulEnergyRegen()
    {
        if (soulEnergyValue < maxSoulEnergyValue)
        {
            soulEnergyValue++;
        }
    }

    public void TakeDamage(int damage)
    {
        soulEnergyValue -= damage;
    }

    public void Heal(int value)
    {
        soulEnergyValue += value;
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        Debug.Log(hitInfo.name);

        if (hitInfo.tag == "DeathZone")
        {
            soulEnergyValue = 0;
        }

        PlayerMovementScript HittedObject1 = hitInfo.GetComponent<PlayerMovementScript>();
        if (HittedObject1 != null && !hadAtacked)
        {
            HittedObject1.TakeDamage(damageValue);
            soulEnergyValue -= AtackConsumation;
            hadAtacked = true;
        }

        HAGScript HittedObject2 = hitInfo.GetComponent<HAGScript>();
        if (HittedObject2 != null && !hadAtacked)
        {
            HittedObject2.TakeDamage(damageValue);
            soulEnergyValue -= AtackConsumation;
            hadAtacked = true;
        }
    }
}
