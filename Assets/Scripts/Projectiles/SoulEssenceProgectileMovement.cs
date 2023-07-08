using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulEssenceProgectileMovement : MonoBehaviour {

    public float speed = 8f;
    public int damageValue = 50;
    public Rigidbody2D rb;

    [SerializeField] private ParticleSystem explosionParticles;

    // Start is called before the first frame update
    void Start ()
    {
        rb.velocity = transform.right * speed;
    }

    void OnTriggerEnter2D (Collider2D hitInfo)
    {
        Debug.Log(hitInfo.name);

        DurabilityScript HittedObject = hitInfo.GetComponent<DurabilityScript>();
        if (HittedObject != null)
        {
            HittedObject.TakeDamage(damageValue);
        }

        PlayerMovementScript HittedObject1 = hitInfo.GetComponent<PlayerMovementScript>();
        if (HittedObject1 != null)
        {
            HittedObject1.TakeDamage(damageValue);
        }

        AwLoSoScript HittedObject2 = hitInfo.GetComponent<AwLoSoScript>();
        if (HittedObject2 != null)
        {
            HittedObject2.TakeDamage(damageValue);
        }

        NegEnScript HittedObject3 = hitInfo.GetComponent<NegEnScript>();
        if (HittedObject3 != null)
        {
            HittedObject3.TakeDamage(damageValue);
        }

        HAGScript HittedObject4 = hitInfo.GetComponent<HAGScript>();
        if (HittedObject4 != null)
        {
            HittedObject4.TakeDamage(damageValue);
        }

        if (hitInfo.CompareTag("SightArea") == false && hitInfo.CompareTag("EnergyEmision") == false)
        {
            if (hitInfo.CompareTag("DeathZone") == false)
            {
                Instantiate(explosionParticles, transform.position, transform.rotation);
                FindObjectOfType<AudioManager>().Play("MGHit");
            }
            Destroy(this.gameObject);
        }
    }

}
