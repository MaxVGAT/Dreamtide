using System.Collections;
using UnityEngine;

// エンティティVFX管理（ダメージ・状態異常・攻撃エフェクト）
public class Entity_VFX : MonoBehaviour
{
    protected SpriteRenderer sr; // スプライト参照
    private Entity entity;

    public enum FlashType { Red, Yellow, Green, White } // ダメージフラッシュ種類

    [Header("On Taking Damage VFX")]
    [SerializeField] private Material interactableHitMat; // インタラクト対象ヒット
    [SerializeField] private Material redHitMat;           // 赤ヒット
    [SerializeField] private Material yellowHitBlockMat;   // 黄ヒット（ブロック）
    [SerializeField] private Material greenHitPerfectBlockMat; // 緑ヒット（パーフェクトブロック）
    [SerializeField] private float onDamageVfxDuration = 0.2f; // ダメージVFX時間
    private Material originalMaterial; // 元のマテリアル保存
    private Coroutine onDamageVfxCoroutine;

    [Header("On Doing Damage VFX")]
    [SerializeField] private Color hitVfxColor = Color.white; // 攻撃VFX色
    [SerializeField] private GameObject hitVfx;              // 通常攻撃VFX
    [SerializeField] private GameObject critHitVfx;          // クリティカルVFX

    [Header("Elements Colors")]
    [SerializeField] private Color chillVfx = Color.cyan;    // 氷属性色
    [SerializeField] private Color burnVfx = Color.red;      // 火属性色
    [SerializeField] private Color shockVfx = Color.yellow;  // 雷属性色
    private Color originalHitVfxColor;

    private void Awake()
    {
        entity = GetComponent<Entity>();
        sr = GetComponentInChildren<SpriteRenderer>();
        originalMaterial = sr.material; // 元マテリアル保存
        originalHitVfxColor = hitVfxColor;
    }

    // 状態異常VFX再生
    public void PlayOnStatusVfx(float duration, ElementType element)
    {
        if (element == ElementType.Ice) StartCoroutine(PlayStatusVfxCo(duration, chillVfx));
        if (element == ElementType.Fire) StartCoroutine(PlayStatusVfxCo(duration, burnVfx));
        if (element == ElementType.Lightning) StartCoroutine(PlayStatusVfxCo(duration, shockVfx));
    }

    // 全VFX停止
    public void StopAllVfx()
    {
        StopAllCoroutines();
        sr.color = Color.white;
        sr.material = originalMaterial;
    }

    // 状態異常VFXコルーチン
    private IEnumerator PlayStatusVfxCo(float duration, Color effectColor)
    {
        float tickInterval = 0.25f;
        float timeHasPassed = 0;
        Color lightColor = effectColor * 1.2f;
        Color darkColor = effectColor * 0.8f;
        bool toggle = false;

        while (timeHasPassed < duration)
        {
            sr.color = toggle ? lightColor : darkColor; // 色交互
            toggle = !toggle;
            yield return new WaitForSeconds(tickInterval);
            timeHasPassed += tickInterval;
        }

        sr.color = Color.white; // 元色に戻す
    }

    // 攻撃時VFX生成
    public void CreateOnHitVFX(Transform target, bool isCrit, ElementType element)
    {
        GameObject hitPrefab = isCrit ? critHitVfx : hitVfx;
        GameObject vfx = Instantiate(hitPrefab, target.position, Quaternion.identity);

        // 左向きクリティカルVFX反転
        if (entity.facingDirection == -1 && isCrit) vfx.transform.Rotate(0, 180, 0);
    }

    // 属性色取得
    public Color GetElementColor(ElementType element)
    {
        switch (element)
        {
            case ElementType.Ice: return chillVfx;
            case ElementType.Fire: return burnVfx;
            case ElementType.Lightning: return shockVfx;
            default: return Color.white;
        }
    }

    // ダメージフラッシュ制御
    public void HandleHitColor(FlashType type)
    {
        Material mat = redHitMat;
        if (type == FlashType.Yellow) mat = yellowHitBlockMat;
        if (type == FlashType.White) mat = interactableHitMat;
        // Greenフラッシュは未使用
        PlayOnDamageVfx(mat);
    }

    // ダメージVFX開始
    public void PlayOnDamageVfx(Material hitMaterial)
    {
        if (onDamageVfxCoroutine != null) StopCoroutine(onDamageVfxCoroutine);
        onDamageVfxCoroutine = StartCoroutine(OnDamageVfxCo(hitMaterial));
    }

    // ダメージVFXコルーチン
    private IEnumerator OnDamageVfxCo(Material hitMaterial)
    {
        sr.material = hitMaterial;
        yield return new WaitForSeconds(onDamageVfxDuration);
        sr.material = originalMaterial; // 元に戻す
    }
}
