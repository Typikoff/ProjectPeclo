using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataPersistence
{
    void LoadData(GameData data); // only reads the data

    void SaveData(ref GameData data); // reference allows modifying data 
}
