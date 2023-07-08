using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour, IDataPersistence
{
    public static int maxSoulEnergyValue = 1000;
    public static int sins = 0;
    public static bool hadGoneFrom1Location = false;
    public static int soulEnergyValue = 100;
    public static bool anger = false;

    // private void Start()
    // {
    //     Debug.Log("Called LoadGame");
    //     DataPersistenceManager.instance.LoadGame();
    // }

    public void LoadData (GameData data)
    {
        // no need to use 'this', it's a static tipe and cann't have an instance
        maxSoulEnergyValue = data.maxSoulEnergyValue;
    }

    public void SaveData (ref GameData data)
    {
        // no need to use 'this', it's a static tipe and cann't have an instance
        data.maxSoulEnergyValue = maxSoulEnergyValue;
    }
}
