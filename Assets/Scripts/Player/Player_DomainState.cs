using UnityEngine;

// プレイヤーのドメイン発動状態
public class Player_DomainState : PlayerState
{
    private Vector2 originalPosition;       // ドメイン開始時のプレイヤー位置
    private float originalGravity;          // ドメイン開始前の重力スケール
    private float maxDistanceToGoUp;        // 上昇可能な最大距離

    private bool isLevitating;              // 浮遊中フラグ
    private bool createdDomain;             // ドメイン生成済みフラグ

    public Player_DomainState(Entity_Player player, StateMachine stateMachine, string animBoolName)
        : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // 初期状態を保存
        originalPosition = player.transform.position;
        originalGravity = rb.gravityScale;
        maxDistanceToGoUp = GetAvailableRiseDistance();

        // 上昇開始
        player.SetVelocity(0, player.riseSpeed);

        // 無敵状態にする
        player.health.SetCanTakeDamage(false);
    }

    public override void Update()
    {
        base.Update();

        // 最大上昇距離に到達したら浮遊開始
        if (Vector2.Distance(originalPosition, player.transform.position) >= maxDistanceToGoUp && !isLevitating)
            Levitate();

        if (isLevitating)
        {
            skillManager.domain.DoSpellCasting();

            // 浮遊時間終了で状態終了
            if (stateTimer < 0)
            {
                rb.gravityScale = originalGravity;
                isLevitating = false;
                stateMachine.ChangeState(player.idleState);
            }
        }
    }

    public override void Exit()
    {
        base.Exit();

        createdDomain = false;
        player.health.SetCanTakeDamage(true); // 無敵解除
    }

    // 浮遊開始処理
    private void Levitate()
    {
        isLevitating = true;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0;

        stateTimer = skillManager.domain.GetDomainDuration();

        if (!createdDomain)
        {
            createdDomain = true;
            skillManager.domain.CreateDomain(); // ドメイン生成
        }
    }

    // 上昇可能距離の計算
    private float GetAvailableRiseDistance()
    {
        RaycastHit2D hit =
            Physics2D.Raycast(player.transform.position, Vector2.up, player.riseMaxDistance, player.whatIsGround);

        return hit.collider != null ? hit.distance - 1 : player.riseMaxDistance;
    }
}
