using UnityEngine;

public class Enemy_AnimationTriggers : Entity_AnimationTriggers // 敵のアニメーションイベントを処理するクラス
{
    private Entity_Enemy enemy; // Entity_Enemy クラスの参照をキャッシュ
    private Enemy_VFX enemyVfx; // 敵のVFXを制御するクラスの参照をキャッシュ

    protected override void Awake() // Entity_AnimationTriggers をオーバーライドしてコンポーネントを取得
    {
        base.Awake();
        enemy = GetComponentInParent<Entity_Enemy>();
        enemyVfx = GetComponentInParent<Enemy_VFX>();
    }

    private void EnableCounterWindow() // プレイヤーが攻撃をブロックできるタイミングのアニメーションイベント
    {
        enemyVfx.EnableAttackAlert(true);
        enemy.EnableCounterAttack(true);
    }

    private void DisableCounterWindow() // 攻撃がブロック不可能になるタイミングのアニメーションイベント
    {
        enemyVfx.EnableAttackAlert(false);
        enemy.EnableCounterAttack(false);
    }
}
