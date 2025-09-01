using System.Collections;
using UnityEngine;

[System.Serializable]
public class Buff
{
    public StatType type;  // 対象のステータス
    public float value;    // バフ量
}

public class Object_ItemEffect : MonoBehaviour
{
    private SpriteRenderer sr;              // 表示用スプライト
    private Entity_Stats statsToModify;     // バフ対象のステータス

    [Header("Buff details")]
    [SerializeField] private Buff[] buffs;        // 適用するバフ一覧
    [SerializeField] private string buffName;     // バフ名（識別用）
    [SerializeField] private float buffDuration = 4f; // バフの持続時間
    [SerializeField] private bool canBeUsed = true;   // 使用可能フラグ

    [Header("Pulse details")]
    [SerializeField] private float pulseSpeed = 1;    // パルス速度
    [SerializeField] private float minScale = 0.8f;  // 最小スケール
    [SerializeField] private float maxScale = 1.2f;  // 最大スケール
    [SerializeField] private float timeOffset = 0f;  // アニメーション位相オフセット

    private Vector3 originalScale;  // 元のスケール

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        originalScale = this.transform.localScale;

        timeOffset = Random.Range(0f, Mathf.PI * 2); // バフごとにアニメーションずらす
    }

    private void Update()
    {
        // パルスアニメーション計算
        float sineValue = Mathf.Sin((Time.time + timeOffset) * pulseSpeed);
        float pulseScale = Mathf.Lerp(minScale, maxScale, (sineValue + 1f) / 2f);

        this.transform.localScale = originalScale * pulseScale;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canBeUsed) return;

        statsToModify = collision.GetComponent<Entity_Stats>();
        StartCoroutine(BuffCo(buffDuration)); // バフ適用開始
    }

    // バフ適用コルーチン
    private IEnumerator BuffCo(float duration)
    {
        canBeUsed = false;
        sr.color = Color.clear;    // 視覚的に消す

        ApplyBuff(true);           // バフ適用

        yield return new WaitForSeconds(duration);

        ApplyBuff(false);          // バフ解除
        Destroy(gameObject);       // オブジェクト破棄
    }

    // バフの適用/解除
    private void ApplyBuff(bool apply)
    {
        foreach (var buff in buffs)
        {
            if (apply)
                statsToModify.GetStatByType(buff.type).AddModifier(buff.value, buffName);
            else
                statsToModify.GetStatByType(buff.type).RemoveModifier(buffName);
        }
    }
}
