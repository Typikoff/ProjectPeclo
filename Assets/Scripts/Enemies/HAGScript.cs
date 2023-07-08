using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HAGScript : MonoBehaviour
{
    public Transform FirePoint;
    public Transform player;
    public Collider2D sightArea;
    public GameObject progectile;
    public Rigidbody2D rb;
    public GameObject afterDeathEmotion;
    public GameObject helmet;
    public GameObject chestplate;
    private Animator HAGAnimator;
    private AudioManager audioManager;

    private int maxSoulEnergyValue = 1000; 
    public int soulEnergyValue = 500; 
    private float fireRate = 2f; // in seconds
    private float canFireInTime = 0f;
    private float timerForArmorPiecesSpawn = 2.2f; // in seconds
    private bool timerNotSetted = true;
    public bool isAttacking = false;
    public float timerForShootAnim = 0f;
    private bool canShoot = true;
    public int shootConsumation = 200;

    private float xDiffirence = 0f;
    private float yDiffirence = 0f;

    [SerializeField] private bool isAngered = false;
    [SerializeField] private bool isFacingRight = true;
    [SerializeField] private ParticleSystem deathExplosionParticles;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        HAGAnimator = GetComponent<Animator>();
        audioManager = FindObjectOfType<AudioManager>();

        if (!isFacingRight) // in Unity you can uncheck this bool to make it face left side without breaking whole thing
        {
            Flip();
            isFacingRight = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isAttacking && Time.time > timerForShootAnim + 0.2f) // for switch beetween attacking and not modes
        {
            isAttacking = false;
            canShoot = true;
        }

        // for proper Animator functioning
        HAGAnimator.SetFloat("SoulEnergy", soulEnergyValue);
        HAGAnimator.SetBool("IsAttacking", isAttacking);
        HAGAnimator.SetBool("IsAngered", isAngered);
    }

    void FixedUpdate()
    {
        IsDead();
        SoulEnergyRegen();
        if (!isAngered)
        {
            AngerCheck();
        }
        else if (canShoot && !isAttacking)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        // shooting logic
        if (soulEnergyValue > shootConsumation) 
        {
            soulEnergyValue -= shootConsumation;

            if (canShoot) // for shooting after animation
            {
                Instantiate(progectile, FirePoint.position, FirePoint.rotation);
                audioManager.Play("JEShot");
                canShoot = false;
                isAttacking = false;
            }
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            if (collision.transform.position.x < transform.position.x && isFacingRight)
            {
                Flip();
            }
            else if (collision.transform.position.x > transform.position.x && !isFacingRight)
            {
                Flip();
            }

            if(Time.time > canFireInTime)
            {
                canFireInTime = fireRate + Time.time;
                Shoot();
            }
        }
        if (collision.tag == "Player" && isAngered)
        {
            if (collision.transform.position.x < transform.position.x && isFacingRight)
            {
                Flip();
            }
            else if (collision.transform.position.x > transform.position.x && !isFacingRight)
            {
                Flip();
            }

            if (!isAttacking && soulEnergyValue > shootConsumation && timerForShootAnim < Time.time) // for switch beetween attacking and not modes
            {
                isAttacking = true;
                timerForShootAnim = Time.time + 1f; 
            }
            
            if(Time.time > canFireInTime && Time.time > timerForShootAnim)
            {
                canFireInTime = fireRate + Time.time;
                Shoot();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "DeathZone")
        {
            soulEnergyValue = 0;
        }
    }

    private void AngerCheck()
    {
        if (PlayerStats.anger)
        {
            isAngered = true;
        }
        else
        {
            xDiffirence = (player.position.x - transform.position.x);
            yDiffirence = (player.position.y - transform.position.y);
            if ((xDiffirence >= -2f && xDiffirence <= 2f) && (yDiffirence >= -1f && yDiffirence <= 1f))
            {
                PlayerStats.anger = true;
                isAngered = true;
                audioManager.Play("Triggered");
            }
        }
    }

    private void IsDead()
    {
        if (soulEnergyValue < 1)
        {
            rb.gravityScale = 0f; // stop from falling
            rb.velocity = new Vector2(0, 0);

            if (timerNotSetted)
            {
                timerForArmorPiecesSpawn += Time.time;
                timerNotSetted = false;
                Instantiate(deathExplosionParticles, transform.position, transform.rotation);
                audioManager.Play("HAGDeath");
            }
        }
        if (Time.time >= timerForArmorPiecesSpawn && !timerNotSetted)
        {
            Instantiate(chestplate, transform.position, transform.rotation);
            Instantiate(helmet, transform.position, transform.rotation);
            Instantiate(afterDeathEmotion, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }

    private void Flip()
    {
        if (isFacingRight)
        {
            isFacingRight = false;
            transform.Rotate(0f, 180f, 0f);
        }
        else
        {
            isFacingRight = true;
            transform.Rotate(0f, 180f, 0f);
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
        PlayerStats.anger = true;
    }
}
