using UnityEngine;

public enum Skill_UpgradeType
{
    None,

    // --- Dash Tree ---
    Dash, // Dash to avoid damage
    Dash_CloneOnStart, // Create a clone when dash starts
    Dash_CloneOnStartAndArrival,
    Dash_ShardOnStart, // Create a time shard when dash starts
    Dash_ShardOnStartAndArrival,

    // --- Shard Tree ---
    Shard, //Shard explodes on enemy contact or after a delay
    Shard_MoveToEnemy, // Shard moves towards nearest enemy
    Shard_MultiCast, // Shard can have multiple charges, and you can cast them all in succession
    Shard_Teleport, // Swap places with the last shard created
    Shard_TeleportHPRewind, // When swapping with the shard, return to the %HP you had when creating the shard

    // --- TimeEcho Tree ---
    TimeEcho, // Create a clone of a player, which can take damage from enemies.
    TimeEcho_SingleAttack, // The clone can perform a single attack
    TimeEcho_MultiAttack, // The clone can perform N attacks
    TimeEcho_ChanceToMultiply, // The clone has a chance to create another clone on attacks
    TimeEcho_HealWisp, // When the clone dies, it creates a wisp that flies towards the player and heal it
    TimeEcho_CleanseWisp, // Wisp now also removes negative effects on player
    TimeEcho_CooldownWisp, // Wisp now reduces cooldown of all skills by N second

    // --- Sword Throw Tree ---
    SwordThrow, // Throw a sword to damage enemies from afar
    SwordThrow_Spin, // Sword will stop at one point and spin to deal damages
    SwordThrow_Pierce, // Sword will pierce N targets
    SwordThrow_Bounce // Sword will bound between enemies
}
