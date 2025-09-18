using UnityEngine;

// エンティティの状態基底クラス
public abstract class EntityState
{
    protected StateMachine stateMachine; // 所属するステートマシン
    protected string animBoolName;       // アニメーションのBoolパラメータ名

    protected Animator anim;             // Animator参照
    protected Rigidbody2D rb;            // Rigidbody2D参照
    protected Entity_Stats stats;        // エンティティのステータス参照

    protected float stateTimer;          // 状態経過時間管理用
    public bool triggerCalled;           // アニメーションのトリガー呼び出しフラグ

    // コンストラクタ：ステートマシンとアニメーションパラメータを設定
    public EntityState(StateMachine stateMachine, string animBoolName)
    {
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }

    // 状態開始時に呼ばれる
    // アニメーションBoolをtrueにしてトリガーフラグをリセット
    public virtual void Enter()
    {
        anim.SetBool(animBoolName, true);
        triggerCalled = false;
    }

    // 毎フレーム更新
    // 状態タイマー減算とアニメーションパラメータ更新
    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;
        UpdateAnimationParameters();
    }

    // 状態終了時に呼ばれる
    // アニメーションBoolをfalseにする
    public virtual void Exit()
    {
        anim.SetBool(animBoolName, false);
    }

    // アニメーションイベント用メソッド
    // 呼ばれるとtriggerCalledをtrueにする
    public void AnimationTrigger()
    {
        triggerCalled = true;
    }

    // 状態中にアニメーションパラメータを更新（必要に応じてオーバーライド）
    public virtual void UpdateAnimationParameters()
    {

    }

    // 攻撃速度とAnimatorの同期
    public void SyncAttackSpeed()
    {
        float attackSpeed = stats.offense.attackSpeed.GetValue();
        anim.SetFloat("attackSpeedMultiplier", attackSpeed);
    }
}
