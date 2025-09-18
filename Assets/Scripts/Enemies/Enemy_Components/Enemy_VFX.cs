using UnityEngine;

public class Enemy_VFX : Entity_VFX // 敵専用VFX管理クラス
{
    [Header("攻撃警告用VFX")]
    [SerializeField] private GameObject attackAlert; // 攻撃対象に向けて警告表示するオブジェクト

    // 攻撃警告の表示/非表示を切り替え
    public void EnableAttackAlert(bool enable) => attackAlert.SetActive(enable);
}
