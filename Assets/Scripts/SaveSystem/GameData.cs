using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class GameData
{
    public int gold;

    public SerializableDictionary<string, int> inventory;


    public GameData()
    {
        inventory = new SerializableDictionary<string, int>();
    }
}