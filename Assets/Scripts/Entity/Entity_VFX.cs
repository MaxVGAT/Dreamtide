using System.Collections;
using UnityEngine;

// エンティティの各種VFX（ダメージ、ステータス、攻撃時）を管理するクラス
public class Entity_VFX : MonoBehaviour
{
    protected SpriteRenderer sr; // スプライト描画用
    private Entity entity;

    public enum FlashType { Red, Yellow, Green, White } // ダメージフラッシュタイプ

    [Header("On Taking Damage VFX")]
    [SerializeField] private Material interactableHitMat;
    [SerializeField] private Material redHitMat;
    [SerializeField] private Material yellowHitBlockMat;
    [SerializeField] private Material greenHitPerfectBlockMat;
    [SerializeField] private float onDamageVfxDuration = 0.2f;
    private Material originalMaterial; // 元のマテリアルを保持
    private Coroutine onDamageVfxCoroutine;

    [Header("On Doing Damage VFX")]
    [SerializeField] private Color hitVfxColor = Color.white;
    [SerializeField] private GameObject hitVfx;
    [SerializeField] private GameObject critHitVfx;

    [Header("Elements Colors")]
    [SerializeField] private Color chillVfx = Color.cyan;
    [SerializeField] private Color burnVfx = Color.red;
    [SerializeField] private Color shockVfx = Color.yellow;
    private Color originalHitVfxColor;

    private void Awake()
    {
        entity = GetComponent<Entity>();
        sr = GetComponentInChildren<SpriteRenderer>();
        originalMaterial = sr.material; // 初期マテリアル保存
        originalHitVfxColor = hitVfxColor;
    }

    // ステータス効果に応じたVFXを再生
    public void PlayOnStatusVfx(float duration, ElementType element)
    {
        if (element == ElementType.Ice)
            StartCoroutine(PlayStatusVfxCo(duration, chillVfx));

        if (element == ElementType.Fire)
            StartCoroutine(PlayStatusVfxCo(duration, burnVfx));

        if (element == ElementType.Lightning)
            StartCoroutine(PlayStatusVfxCo(duration, shockVfx));
    }

    // 全VFX停止（フラッシュやステータス色をリセット）
    public void StopAllVfx()
    {
        StopAllCoroutines();
        sr.color = Color.white;
        sr.material = originalMaterial;
    }

    // ステータスVFXのコルーチン（色の点滅）
    private IEnumerator PlayStatusVfxCo(float duration, Color effectColor)
    {
        float tickInterval = 0.25f; // 点滅間隔
        float timeHasPassed = 0;

        Color lightColor = effectColor * 1.2f; // 明るい色
        Color darkColor = effectColor * 0.8f;   // 暗い色

        bool toggle = false;

        while (timeHasPassed < duration)
        {
            sr.color = toggle ? lightColor : darkColor; // 交互に色変更
            toggle = !toggle;

            yield return new WaitForSeconds(tickInterval);
            timeHasPassed += tickInterval;
        }

        sr.color = Color.white; // 終了時に元色へ
    }

    // 攻撃時VFX生成（通常 or クリティカル）
    public void CreateOnHitVFX(Transform target, bool isCrit, ElementType element)
    {
        GameObject hitPrefab = isCrit ? critHitVfx : hitVfx;
        GameObject vfx = Instantiate(hitPrefab, target.position, Quaternion.identity);

        // 反転処理（左向きのときクリティカルVFXを反転）
        if (entity.facingDirection == -1 && isCrit)
            vfx.transform.Rotate(0, 180, 0);
    }

    // 属性に応じたVFX色を取得
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

    // ダメージ時の色フラッシュ
    public void HandleHitColor(FlashType type)
    {
        Material mat = redHitMat;

        if (type == FlashType.Yellow)
            mat = yellowHitBlockMat;
        if (type == FlashType.White)
            mat = interactableHitMat;
        //else if (type == FlashType.Green)
        //    mat = greenHitPerfectBlockMat;

        PlayOnDamageVfx(mat);
    }

    // ダメージVFX再生（マテリアル変更）
    public void PlayOnDamageVfx(Material hitMaterial)
    {
        if (onDamageVfxCoroutine != null)
            StopCoroutine(onDamageVfxCoroutine); // 既存コルーチン停止

        onDamageVfxCoroutine = StartCoroutine(OnDamageVfxCo(hitMaterial));
    }

    // ダメージVFXコルーチン（指定時間だけフラッシュ）
    private IEnumerator OnDamageVfxCo(Material hitMaterial)
    {
        sr.material = hitMaterial; // フラッシュ用マテリアルに変更

        yield return new WaitForSeconds(onDamageVfxDuration);

        sr.material = originalMaterial; // 終了時に元マテリアルへ戻す
    }
}
