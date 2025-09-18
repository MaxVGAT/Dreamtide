// プレイヤー専用の状態基底クラス
public abstract class PlayerState : EntityState
{
    protected Entity_Player player;           // プレイヤー本体参照
    protected PlayerInputSet input;           // プレイヤー入力参照
    protected Player_SkillManager skillManager; // プレイヤースキル管理参照

    // コンストラクタ：プレイヤーとステートマシン、アニメーションパラメータ設定
    public PlayerState(Entity_Player player, StateMachine stateMachine, string animBoolName)
        : base(stateMachine, animBoolName)
    {
        this.player = player;

        anim = player.anim;
        rb = player.rb;
        input = player.input;
        stats = player.stats;
        skillManager = player.skillManager;
    }

    // 毎フレーム更新
    public override void Update()
    {
        base.Update();

        // ダッシュ入力チェック
        if (input.Player.Dash.WasPressedThisFrame() && CanDash())
        {
            skillManager.dash.SetSkillOnCooldown();   // ダッシュスキルクールタイム設定
            stateMachine.ChangeState(player.dashState); // ダッシュ状態に遷移
        }

        // アルティメットスキル入力チェック
        if (input.Player.UltimateSkill.WasPressedThisFrame() && skillManager.domain.CanUseSkill())
        {
            if (skillManager.domain.InstantDomain()) // 即発動可能か
            {
                skillManager.domain.CreateDomain();   // ドメイン生成
            }
            else
                stateMachine.ChangeState(player.domainState); // 非即発動時はドメイン状態に遷移

            skillManager.domain.SetSkillOnCooldown(); // ドメインスキルクールタイム設定
        }
    }

    // アニメーションパラメータ更新
    public override void UpdateAnimationParameters()
    {
        base.UpdateAnimationParameters();
        anim.SetFloat("yVelocity", rb.linearVelocity.y); // Y軸速度をAnimatorに反映
    }

    // ダッシュ可能判定
    private bool CanDash()
    {
        if (!skillManager.dash.CanUseSkill())   // スキル使用不可
            return false;

        if (player.isWallDetected)               // 壁検知中は不可
            return false;

        if (stateMachine.currentState == player.dashState || stateMachine.currentState == player.domainState)
            return false;                        // ダッシュ中またはドメイン中は不可

        return true;
    }
}
