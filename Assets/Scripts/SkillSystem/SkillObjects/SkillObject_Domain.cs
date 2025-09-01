using Unity.VisualScripting;
using UnityEngine;

public class SkillObject_Domain : SkillObject_Base
{
    private Skill_Domain domainManager; // ドメインの親スキル
    private float expansionSpeed;       // ドメインの拡大速度
    private float slowDownPercent;      // 敵の移動速度減少率
    private float duration;             // ドメイン持続時間

    private Vector3 targetScale;        // 目標スケール（拡大／縮小）
    private bool isShrinking;           // 縮小中か

    /// <summary>
    /// ドメイン初期設定
    /// </summary>
    public void SetupDomain(Skill_Domain domainManager)
    {
        this.domainManager = domainManager;

        duration = domainManager.GetDomainDuration();
        float maxSize = domainManager.maxDomainSize;
        slowDownPercent = domainManager.GetSlowPercentage();
        expansionSpeed = domainManager.expandSpeed;

        targetScale = Vector3.one * maxSize;
        // duration後にドメイン縮小開始
        Invoke(nameof(ShrinkDomain), duration);
    }

    private void Update()
    {
        HandleScaling(); // 拡大・縮小処理
    }

    /// <summary>
    /// ドメインのスケーリング処理
    /// </summary>
    private void HandleScaling()
    {
        float sizeDifference = Mathf.Abs(transform.localScale.x - targetScale.x);
        bool shouldChangeScale = sizeDifference > 0.1f; // 目標スケールとの差が十分にあるか

        if (shouldChangeScale)
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, expansionSpeed * Time.deltaTime);

        // 縮小完了時に終了
        if (isShrinking && sizeDifference < 0.1f)
            TerminateDomain();
    }

    /// <summary>
    /// ドメイン終了処理
    /// </summary>
    private void TerminateDomain()
    {
        domainManager.ClearTargets(); // ターゲットリストクリア
        Destroy(gameObject);          // 自身削除
    }

    /// <summary>
    /// ドメイン縮小開始
    /// </summary>
    private void ShrinkDomain()
    {
        targetScale = Vector3.zero;
        isShrinking = true;
    }

    /// <summary>
    /// ドメイン内に敵が入った
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Entity_Enemy enemy = collision.GetComponent<Entity_Enemy>();
        if (enemy == null) return;

        domainManager.AddTarget(enemy); // ターゲット登録
        enemy.SlowDownEntityBy(duration, slowDownPercent, true); // 移動速度低下
    }

    /// <summary>
    /// ドメインから敵が出た
    /// </summary>
    private void OnTriggerExit2D(Collider2D collision)
    {
        Entity_Enemy enemy = collision.GetComponent<Entity_Enemy>();
        if (enemy == null) return;

        enemy.StopSlowDown(); // 移動速度回復
    }
}
