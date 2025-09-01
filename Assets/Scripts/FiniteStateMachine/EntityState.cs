using UnityEngine;

public abstract class EntityState
{
    protected StateMachine stateMachine; // ステートマシンへの参照
    protected string animBoolName;       // このステートに対応するアニメーションのBool名

    protected Animator anim;             // アニメーターへの参照
    protected Rigidbody2D rb;            // Rigidbody2Dへの参照
    protected Entity_Stats stats;        // エンティティのステータス情報

    protected float stateTimer;          // ステート内での経過時間管理
    protected bool triggerCalled;        // アニメーションイベントの発火管理フラグ

    // コンストラクタ：ステートマシンとアニメーション名を初期化
    public EntityState(StateMachine stateMachine, string animBoolName)
    {
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }

    // ステートに入ったときに呼ばれる
    // アニメーションのBoolをtrueにし、トリガーフラグをリセット
    public virtual void Enter()
    {
        anim.SetBool(animBoolName, true);
        triggerCalled = false;
    }

    // 毎フレーム更新
    // タイマーを減らし、アニメーションパラメータを更新
    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;
        UpdateAnimationParameters();
    }

    // ステートを抜けるときに呼ばれる
    // アニメーションのBoolをfalseにする
    public virtual void Exit()
    {
        anim.SetBool(animBoolName, false);
    }

    // アニメーションイベントから呼ばれる
    // トリガーフラグをtrueに設定
    public void AnimationTrigger()
    {
        triggerCalled = true;
    }

    // ステート固有のアニメーションパラメータ更新処理
    public virtual void UpdateAnimationParameters()
    {

    }

    // 攻撃速度をアニメーターに同期
    public void SyncAttackSpeed()
    {
        float attackSpeed = stats.offense.attackSpeed.GetValue();
        anim.SetFloat("attackSpeedMultiplier", attackSpeed);
    }
}
