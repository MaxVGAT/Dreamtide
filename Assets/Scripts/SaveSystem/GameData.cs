using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class GameData
{
    public int gold;

    public SerializableDictionary<string, int> inventory; // itemSaveID -> stackSize
    public SerializableDictionary<string, int> storageItems;

    public SerializableDictionary<string, Item_Type> equippedItems; // SlotType -> itemSaveID;

    public int skillPoints;
    public SerializableDictionary<string, bool> skillTreeUI; // skillName -> unlock status
    public SerializableDictionary<Skill_Type, Skill_UpgradeType> skillUpgrades; // skill type -> upgrade type

    public SerializableDictionary<string, bool> unlockedCheckpoints; // checkpoint id -> unlocked status

    public int playerLevel;
    public int totalExperience;

    public Vector3 savedCheckpoint;


    public GameData()
    {
        inventory = new SerializableDictionary<string, int>();
        storageItems = new SerializableDictionary<string, int>();

        equippedItems = new SerializableDictionary<string, Item_Type>();

        skillTreeUI = new SerializableDictionary<string, bool>();
        skillUpgrades = new SerializableDictionary<Skill_Type, Skill_UpgradeType>();

        unlockedCheckpoints = new SerializableDictionary<string, bool>();

        playerLevel = 0;
        totalExperience = 0;
    }
}