using UnityEngine;

public class StateMachine
{
    public EntityState currentState { get; private set; } // 現在のステート
    public bool canChangeState = true;                   // ステート変更可能かどうか

    // 初期化：開始ステートを設定してEnter呼び出し
    public void Initialize(EntityState startState)
    {
        currentState = startState;
        currentState.Enter(); // ステート開始処理
    }

    // ステート切り替え
    public void ChangeState(EntityState newState)
    {
        if (canChangeState == false) return; // ステート変更不可なら処理しない
        currentState.Exit();                 // 現在のステート終了処理
        currentState = newState;             // 新しいステートに変更
        currentState.Enter();                // 新ステート開始処理
    }

    // 現在アクティブなステートの更新
    public void UpdateActiveState()
    {
        currentState.Update();               // Update呼び出し
    }

    // ステートマシンを停止（ステート変更不可）
    public void SwitchOffStateMachine() => canChangeState = false;
}
