// 状態マシン管理クラス
public class StateMachine
{
    public EntityState currentState { get; private set; } // 現在の状態
    public bool canChangeState = true;                   // 状態変更可能か

    // 初期状態を設定してEnter呼び出し
    public void Initialize(EntityState startState)
    {
        currentState = startState;
        currentState.Enter(); // 状態開始処理
    }

    // 状態変更処理
    public void ChangeState(EntityState newState)
    {
        if (canChangeState == false) return; // 変更不可なら何もしない
        currentState.Exit();                 // 現在状態の終了処理
        currentState = newState;             // 新しい状態に切替
        currentState.Enter();                // 新状態の開始処理
    }

    // 毎フレームの状態更新
    public void UpdateActiveState()
    {
        currentState.Update();               // Update呼び出し
    }

    // 状態変更禁止
    public void SwitchOffStateMachine() => canChangeState = false;
}
