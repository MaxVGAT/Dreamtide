// アイテム取得時のバフエフェクト
using UnityEngine;

public class Object_ItemEffect : MonoBehaviour
{
    private Player_Stats statsToModify;     // バフを適用するプレイヤーステータス

    [Header("Buff details")]
    [SerializeField] private BuffEffectData[] buffs;        // 適用するバフ情報
    [SerializeField] private string buffName;               // バフ名識別用
    [SerializeField] private float buffDuration = 4f;       // バフ持続時間

    [Header("Pulse details")]
    [SerializeField] private float pulseSpeed = 1;    // 拡縮のスピード
    [SerializeField] private float minScale = 0.8f;  // 最小スケール
    [SerializeField] private float maxScale = 1.2f;  // 最大スケール
    [SerializeField] private float timeOffset = 0f;  // パルスの初期位相

    private Vector3 originalScale;  // 元のスケール

    private void Awake()
    {
        originalScale = this.transform.localScale;
        timeOffset = Random.Range(0f, Mathf.PI * 2); // ランダムに位相をずらす
    }

    private void Update()
    {
        // パルス拡縮処理
        float sineValue = Mathf.Sin((Time.time + timeOffset) * pulseSpeed);
        float pulseScale = Mathf.Lerp(minScale, maxScale, (sineValue + 1f) / 2f);

        this.transform.localScale = originalScale * pulseScale;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        statsToModify = collision.GetComponent<Player_Stats>();

        if (statsToModify.CanApplyBuffOf(buffName))
        {
            statsToModify.ApplyBuff(buffs, buffDuration, buffName); // バフ適用
            Destroy(gameObject); // エフェクト削除
        }
    }
}
