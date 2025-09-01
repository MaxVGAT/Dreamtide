using UnityEngine;

public interface ICounterable
{
    // カウンター可能かどうか
    public bool CanBeCountered { get; }

    // カウンター攻撃を処理
    public void HandleCounterAttack();
}