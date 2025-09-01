public abstract class PlayerState : EntityState
{
    protected Entity_Player player;           // プレイヤー本体への参照
    protected PlayerInputSet input;           // プレイヤーの入力情報
    protected Player_SkillManager skillManager; // プレイヤーのスキル管理

    // コンストラクタ：プレイヤーとステートマシン、アニメーション名を初期化
    public PlayerState(Entity_Player player, StateMachine stateMachine, string animBoolName) : base(stateMachine, animBoolName)
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

        // ダッシュ入力処理
        if (input.Player.Dash.WasPressedThisFrame() && CanDash())
        {
            skillManager.dash.SetSkillOnCooldown(); // ダッシュスキルをクールダウンに設定
            stateMachine.ChangeState(player.dashState); // ダッシュステートへ切り替え
        }

        // アルティメットスキル入力処理
        if (input.Player.UltimateSkill.WasPressedThisFrame() && skillManager.domain.CanUseSkill())
        {
            if (skillManager.domain.InstantDomain()) // 即時発動可能なら
            {
                skillManager.domain.CreateDomain();   // ドメインを生成
            }
            else
                stateMachine.ChangeState(player.domainState); // そうでなければステート切り替え

            skillManager.domain.SetSkillOnCooldown(); // スキルをクールダウンに設定
        }
    }

    // アニメーションパラメータ更新
    public override void UpdateAnimationParameters()
    {
        base.UpdateAnimationParameters();
        anim.SetFloat("yVelocity", rb.linearVelocity.y); // Y軸速度をアニメーターに反映
    }

    // ダッシュ可能か判定
    private bool CanDash()
    {
        if (!skillManager.dash.CanUseSkill())   // スキル使用不可
            return false;

        if (player.isWallDetected)               // 壁接触中は不可
            return false;

        if (stateMachine.currentState == player.dashState || stateMachine.currentState == player.domainState)
            return false;                        // 既にダッシュ中やドメイン中は不可

        return true;
    }
}
