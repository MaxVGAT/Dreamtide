using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class GameData
{
    public int gold;

    public SerializableDictionary<string, int> inventory; // itemSaveID -> stackSize
    public SerializableDictionary<string, int> storageItems;

    public SerializableDictionary<string, Item_Type> equippedItems;


    public GameData()
    {
        inventory = new SerializableDictionary<string, int>();
        storageItems = new SerializableDictionary<string, int>();

        equippedItems = new SerializableDictionary<string, Item_Type>();
    }
}