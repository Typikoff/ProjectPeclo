using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulStrikeWeapon : MonoBehaviour
{

    public Transform FirePoint;
    public GameObject progectile;
    public int shootConsumation = 100;
    private PlayerMovementScript playerMovementScript;
    private AudioManager audioManager;

    void Start()
    {
        playerMovementScript = GetComponent<PlayerMovementScript>();
        audioManager = FindObjectOfType<AudioManager>();
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
        
    }

    void Shoot ()
    {
        // shooting logic
        PlayerStats.soulEnergyValue -= shootConsumation;
        if (PlayerStats.soulEnergyValue >= 0) // >= enables so called "Last Shot", which is fatal for player but also shoots
        {
            Instantiate(progectile, FirePoint.position, FirePoint.rotation);
            audioManager.Play("MGShoot");
        }
        else
        {
            PlayerStats.soulEnergyValue = 0;
        }
    }
}
