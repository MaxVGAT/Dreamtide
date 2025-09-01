using UnityEngine;

public class Skill_SwordThrow : Skill_Base
{
    private SkillObject_Sword currentSword; // 現在投げている剣
    private float currentThrowPower;        // 現在のスロー強度

    [Header("Regular Sword Upgrade")]
    [SerializeField] private GameObject swordPrefab; // 通常剣のプレハブ
    [Range(0, 10)]
    [SerializeField] private float regularThrowPower = 5;

    [Header("Pierce Sword Upgrade")]
    [SerializeField] private GameObject pierceSwordPrefab; // 貫通剣
    public int amountToPierce = 2; // 貫通回数
    [Range(0, 10)]
    [SerializeField] private float pierceThrowPower = 5;

    [Header("Spin Sword Upgrade")]
    [SerializeField] private GameObject spinSwordPrefab;
    [Range(0, 10)]
    [SerializeField] private float spinThrowPower = 5;
    public int maxDistance = 5; // 回転剣の最大距離
    public float attacksPerSecond = 2; // 攻撃頻度
    public float maxSpinDuration = 3; // 最大回転時間

    [Header("Bounce Sword Upgrade")]
    [SerializeField] private GameObject bounceSwordPrefab;
    [Range(0, 10)]
    [SerializeField] private float bounceThrowPower = 5;
    public int bounceCount = 5; // バウンス回数
    public float bounceSpeed = 12; // バウンス速度

    [Header("Trajectory Prediction")]
    [SerializeField] private GameObject predictionDot; // 予測表示用ドット
    [SerializeField] private int numberOfDots = 20;    // ドット数
    [SerializeField] private float spaceBetweenDots = 0.05f; // ドット間隔
    private float swordGravity;  // 剣にかかる重力スケール
    private Transform[] dots;    // 軌道予測ドット
    private Vector2 confirmedDirection; // 投擲確定方向

    protected override void Awake()
    {
        base.Awake();
        swordGravity = swordPrefab.GetComponent<Rigidbody2D>().gravityScale; // 剣の重力スケール取得
        dots = GenerateDots(); // 軌道予測ドット生成
    }

    // スキル使用可能か判定
    public override bool CanUseSkill()
    {
        UpdateThrowPower(); // 現在のアップグレードに応じた投擲力更新

        if (currentSword != null)
        {
            currentSword.GetSwordBackToPlayer(); // 既存剣を戻す
            return false;
        }

        return base.CanUseSkill();
    }

    // 剣を投げる
    public void ThrowSword()
    {
        GameObject swordPrefab = GetSwordPrefab();
        GameObject newSword = Instantiate(swordPrefab, dots[1].position, Quaternion.identity);

        currentSword = newSword.GetComponent<SkillObject_Sword>();
        currentSword.SetupSword(this, GetThrowPower());

        SetSkillOnCooldown(); // 使用後にクールダウン開始
    }

    // 現在のアップグレードに応じた剣プレハブを返す
    private GameObject GetSwordPrefab()
    {
        if (Unlocked(Skill_UpgradeType.SwordThrow))
            return swordPrefab;

        if (Unlocked(Skill_UpgradeType.SwordThrow_Pierce))
            return pierceSwordPrefab;

        if (Unlocked(Skill_UpgradeType.SwordThrow_Spin))
            return spinSwordPrefab;

        if (Unlocked(Skill_UpgradeType.SwordThrow_Bounce))
            return bounceSwordPrefab;

        return null;
    }

    // 現在のアップグレードに応じて投擲力を設定
    private void UpdateThrowPower()
    {
        switch (upgradeType)
        {
            case Skill_UpgradeType.SwordThrow:
                currentThrowPower = regularThrowPower;
                break;
            case Skill_UpgradeType.SwordThrow_Pierce:
                currentThrowPower = pierceThrowPower;
                break;
            case Skill_UpgradeType.SwordThrow_Spin:
                currentThrowPower = spinThrowPower;
                break;
            case Skill_UpgradeType.SwordThrow_Bounce:
                currentThrowPower = bounceThrowPower;
                break;
            default:
                Debug.Log("No upgrade unlocked");
                break;
        }
    }

    // 投擲速度ベクトルを計算
    private Vector2 GetThrowPower() => confirmedDirection * (currentThrowPower * 10);

    // 軌道予測更新
    public void PredictTrajectory(Vector2 direction)
    {
        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].position = GetTrajectoryPoint(direction, i * spaceBetweenDots);
        }
    }

    // 時間t後の予測位置を計算
    private Vector2 GetTrajectoryPoint(Vector2 direction, float t)
    {
        float scaledThrowPower = currentThrowPower * 10;
        Vector2 initialVelocity = direction * scaledThrowPower; // 初速ベクトル
        Vector2 gravityEffect = 0.5f * Physics2D.gravity * swordGravity * (t * t); // 重力による位置変化
        Vector2 predictedPoint = (initialVelocity * t) + gravityEffect; // 合成位置
        Vector2 playerPosition = transform.root.position;

        return playerPosition + predictedPoint;
    }

    public void ConfirmTrajectory(Vector2 direction) => confirmedDirection = direction;

    // ドットの表示切替
    public void EnableDots(bool enable)
    {
        foreach (Transform t in dots)
            t.gameObject.SetActive(enable);
    }

    // ドット生成
    private Transform[] GenerateDots()
    {
        Transform[] newDots = new Transform[numberOfDots];

        for (int i = 0; i < numberOfDots; i++)
        {
            newDots[i] = Instantiate(predictionDot, transform.position, Quaternion.identity, transform).transform;
            newDots[i].gameObject.SetActive(false);
        }

        return newDots;
    }
}
