using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpEssenceAttack : MonoBehaviour
{
    public int damageValue = 250;
    private float atackCooldown = 0.5f;
    private float timeOfNextAtack = 0f;
    private bool timerSettedUp = false;
    private bool hadAtacked = false;
    private AudioManager audioManager;

    [SerializeField] private ParticleSystem attackParticles;

    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    void FixedUpdate()
    {
        AttackUpdate();
    }

    void AttackUpdate() // just a cooldown
    {
        if (hadAtacked)
        {
            if (!timerSettedUp)
            {
                timeOfNextAtack = Time.time + atackCooldown;
                timerSettedUp = true;
                audioManager.Play("JEHit");
            }
            if (Time.time >= timeOfNextAtack)
            {
                hadAtacked = false;
                timerSettedUp = false;
            }
        }
    }

    void OnTriggerEnter2D (Collider2D hitInfo)
    {
        Debug.Log(hitInfo.name);

        if (!(hitInfo.tag == "SightArea"))
        {
            Instantiate(attackParticles, transform.position, transform.rotation);
        }

        DurabilityScript HittedObject = hitInfo.GetComponent<DurabilityScript>();
        if (HittedObject != null && !hadAtacked)
        {
            HittedObject.TakeDamage(damageValue);
            hadAtacked = true;
        }

        PlayerMovementScript HittedObject1 = hitInfo.GetComponent<PlayerMovementScript>();
        if (HittedObject1 != null && !hadAtacked)
        {
            HittedObject1.TakeDamage(damageValue);
            hadAtacked = true;           
        }

        AwLoSoScript HittedObject2 = hitInfo.GetComponent<AwLoSoScript>();
        if (HittedObject2 != null && !hadAtacked)
        {
            HittedObject2.TakeDamage(damageValue);
            hadAtacked = true; 
        }

        NegEnScript HittedObject3 = hitInfo.GetComponent<NegEnScript>();
        if (HittedObject3 != null && !hadAtacked)
        {
            HittedObject3.TakeDamage(damageValue);
            hadAtacked = true; 
        }
    }
}
