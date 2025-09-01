using UnityEngine;

// プレイヤー用アニメーショントリガー
public class Player_AnimationTrigger : Entity_AnimationTriggers
{
    private Entity_Player player; // 親プレイヤー参照

    protected override void Awake()
    {
        base.Awake();
        player = GetComponentInParent<Entity_Player>(); // 親からプレイヤー取得
    }

    // アニメーションイベント用: 剣を投げる
    private void ThrowSword() => player.skillManager.swordThrow.ThrowSword();
}
