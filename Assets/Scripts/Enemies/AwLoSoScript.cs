using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwLoSoScript : MonoBehaviour
{

    public Transform FirePoint;
    public Collider2D FrontSight;
    public Collider2D BackSight;
    public GameObject progectile;
    public Rigidbody2D rigidBodyComponent;

    private float speed = 3f;
    private int maxSoulEnergyValue = 550;
    public int soulEnergyValue = 220;
    private int shootConsumation = 100;
    private int twoFrameCounter = 0;
    private float fireRate = 0.5f; // in seconds
    private float canFireInTime = 0f;
    private bool deathEmotionNotSpawned = true;

    private Animator LostSoulAnimator;
    public GameObject afterDeathEmotion;
    private AudioManager audioManager;

    [SerializeField] private ParticleSystem deathExplosionParticles;
    [SerializeField] private bool isFacingRight = true;

    // Start is called before the first frame update
    void Start()
    {
        rigidBodyComponent = GetComponent<Rigidbody2D>();
        LostSoulAnimator = GetComponent<Animator>();
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
        LostSoulAnimator.SetFloat("SoulEnergy", soulEnergyValue);
        if (soulEnergyValue > maxSoulEnergyValue)
        {
            LostSoulAnimator.SetBool("IsOverCharged", true);
        }
        else
        {
            LostSoulAnimator.SetBool("IsOverCharged", false);
        }
    }

    void FixedUpdate()
    {
        IsDead();
        SoulEnergyRegen();
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player" || collision.tag == "Neutrals")
        {
            if(Time.time > canFireInTime)
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

        if (collision.tag == "Player")
        {
            if (collision.transform.position.x < transform.position.x && isFacingRight)
            {
                Flip();
            }
            else if (collision.transform.position.x > transform.position.x && !isFacingRight)
            {
                Flip();
            } 
        }
    }

    private void IsDead()
    {
        if (soulEnergyValue < 1)
        {
            if (deathEmotionNotSpawned)
            {
                Instantiate(afterDeathEmotion, transform.position, transform.rotation);
                Instantiate(deathExplosionParticles, transform.position, transform.rotation);
                audioManager.Play("AwLoSoDeath");
                deathEmotionNotSpawned = false;
                rigidBodyComponent.mass = 100;
            }
            Destroy(gameObject, 1f);
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
            twoFrameCounter++;
            if(twoFrameCounter >= 2)
            {
                twoFrameCounter = 0;
                soulEnergyValue++;
            }
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

    private void Shoot()
    {
        // shooting logic
        if (soulEnergyValue > shootConsumation) 
        {
            soulEnergyValue -= shootConsumation;
            Instantiate(progectile, FirePoint.position, FirePoint.rotation);
            audioManager.Play("LSEPShot");
        }
        else 
        {
            Panic();
        }
    }

    private void Panic()
    {
        if (isFacingRight)
        {
        rigidBodyComponent.velocity = new Vector2(speed / -2, rigidBodyComponent.velocity.y);
        }
        else
        {
            rigidBodyComponent.velocity = new Vector2(speed / 2, rigidBodyComponent.velocity.y);
        }
    }

}
