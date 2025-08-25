using UnityEngine;

public class Enemy_VFX : Entity_VFX // 攻撃ターゲットのための小さな補助クラス
{
    [Header("カウンター攻撃ウィンドウのVFX")]
    [SerializeField] private GameObject attackAlert; // 攻撃警告用のゲームオブジェクトを割り当てる

    // 敵が攻撃を仕掛ける際に攻撃警告を表示・非表示に切り替える
    public void EnableAttackAlert(bool enable) => attackAlert.SetActive(enable);
}
