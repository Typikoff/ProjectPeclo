using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DurabilityScript : MonoBehaviour
{
   public int durability = 100;

   public void TakeDamage (int damage)
   {
    durability -= damage;

    if (durability <= 0)
    {
        Demolish();
    }
   }

   void Demolish()
   {
    Destroy(gameObject);
   }
}
