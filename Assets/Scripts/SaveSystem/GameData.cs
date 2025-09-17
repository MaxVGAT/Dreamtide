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

    public int playerLevel;
    public int totalExperience;

    public string lastScenePlayed;

    public List<string> openedChests; // chestID -> opened status


    public GameData()
    {
        gold = 10000;

        inventory = new SerializableDictionary<string, int>();
        storageItems = new SerializableDictionary<string, int>();

        equippedItems = new SerializableDictionary<string, Item_Type>();

        skillTreeUI = new SerializableDictionary<string, bool>();
        skillUpgrades = new SerializableDictionary<Skill_Type, Skill_UpgradeType>();

        openedChests = new List<string>(); // initialize the list

        playerLevel = 0;
        totalExperience = 0;
    }
}