using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
// キャラクターやスキルのステータス管理用クラス
public class Stats
{
    [SerializeField] public float baseValue; // ステータスの基本値
    [SerializeField] private List<StatModifier> modifiers = new List<StatModifier>(); // 付加効果（装備やバフなど）一覧

    private bool wasModified = true; // ステータス値が更新されたかどうかのフラグ
    private float finalValue;        // 計算後の最終ステータス値

    // 現在のステータス値を取得（変更がある場合は再計算）
    public float GetValue()
    {
        if (wasModified)
        {
            finalValue = GetFinalValue(); // 修飾子を含めた最終値を計算
            wasModified = false;
        }

        return finalValue;
    }

    // 新しい修飾子（バフ、装備効果など）を追加
    public void AddModifier(float value, string source)
    {
        StatModifier modToAdd = new StatModifier(value, source);
        modifiers.Add(modToAdd);
        wasModified = true; // ステータス更新フラグを立てる
    }

    // 特定のソースの修飾子を削除
    public void RemoveModifier(string source)
    {
        modifiers.RemoveAll(mod => mod.source == source);
        wasModified = true; // ステータス更新フラグを立てる
    }

    // 修飾子を含めた最終ステータス値を計算
    private float GetFinalValue()
    {
        float finalValue = baseValue;

        foreach (var mod in modifiers)
        {
            finalValue += mod.value; // 修飾子の値を加算
        }

        return finalValue;
    }

    // 基本値を直接設定
    public void SetBaseValue(float value) => baseValue = value;
}

[Serializable]
// ステータスの修飾子を表すクラス（加算のみ）
public class StatModifier
{
    public float value;   // 修飾値
    public string source; // 修飾の由来（装備名やバフ名など）

    public StatModifier(float value, string source)
    {
        this.value = value;
        this.source = source;
    }
}
