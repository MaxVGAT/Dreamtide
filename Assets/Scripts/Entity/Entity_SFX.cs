using UnityEngine;

// エンティティ専用のサウンド再生管理
public class Entity_SFX : MonoBehaviour
{
    private AudioSource audioSource; // AudioSource参照

    [Header("SFX Names")]
    [SerializeField] private string attackHit; // 攻撃ヒット音
    [SerializeField] private string attackMiss; // 攻撃ミス音
    [Space]
    [SerializeField] private float soundDistance = 10f; // 音の距離減衰範囲
    [SerializeField] private bool showGizmo; // ギズモ表示フラグ

    private void Awake()
    {
        audioSource = GetComponentInChildren<AudioSource>(); // 子オブジェクトからAudioSource取得
    }

    // 攻撃ヒット音再生
    public void PlayAttackHit()
    {
        SoundManager.instance.PlaySFX(attackHit, audioSource, true, soundDistance);
    }

    // 攻撃ミス音再生
    public void PlayAttackMiss()
    {
        SoundManager.instance.PlaySFX(attackMiss, audioSource, true, soundDistance);
    }

    // 音範囲ギズモ表示
    private void OnDrawGizmos()
    {
        if (showGizmo)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, soundDistance);
        }
    }
}
