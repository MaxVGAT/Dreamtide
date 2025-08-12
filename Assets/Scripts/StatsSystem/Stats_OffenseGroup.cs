using UnityEngine;
using System;

[Serializable]

public class Stats_OffenseGroup
{

    public Stats attackSpeed;

    //Physical stats
    public Stats damage;
    public Stats critPower;
    public Stats critChance;
    public Stats armorReduction;


    //Elemental damages
    public Stats fireDamage;
    public Stats iceDamage;
    public Stats lightningDamage;
}
