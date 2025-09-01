using UnityEngine;

public class SkillObject_SwordPierce : SkillObject_Sword
{
    private int amountToPierce; // 残り貫通可能回数

    public override void SetupSword(Skill_SwordThrow manager, Vector2 direction)
    {
        base.SetupSword(manager, direction);

        amountToPierce = manager.amountToPierce; // マネージャから貫通回数を取得
    }

    // 衝突時の処理
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        bool groundHit = collision.gameObject.layer == LayerMask.NameToLayer("Ground");

        // 地面に当たった、または貫通回数が0なら剣を停止
        if (amountToPierce <= 0 || groundHit)
        {
            DamageEnemiesInRadius(transform, 0.3f); // 衝突時に範囲攻撃
            StopSword(collision);                   // 剣を停止させて親を設定
            return;
        }

        amountToPierce--;                            // 貫通回数を減らす
        DamageEnemiesInRadius(transform, 0.3f);      // 衝突時に範囲攻撃
    }
}
