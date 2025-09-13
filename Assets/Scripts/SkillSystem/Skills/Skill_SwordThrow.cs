using UnityEngine;

public class Skill_SwordThrow : Skill_Base
{
    private SkillObject_Sword currentSword; // ���ݓ����Ă��錕
    private float currentThrowPower;        // ���݂̃X���[���x

    [Header("Regular Sword Upgrade")]
    [SerializeField] private GameObject swordPrefab; // �ʏ팕�̃v���n�u
    [Range(0, 10)]
    [SerializeField] private float regularThrowPower = 5;

    [Header("Pierce Sword Upgrade")]
    [SerializeField] private GameObject pierceSwordPrefab; // �ђʌ�
    public int amountToPierce = 2; // �ђʉ�
    [Range(0, 10)]
    [SerializeField] private float pierceThrowPower = 5;

    [Header("Spin Sword Upgrade")]
    [SerializeField] private GameObject spinSwordPrefab;
    [Range(0, 10)]
    [SerializeField] private float spinThrowPower = 5;
    public int maxDistance = 5; // ��]���̍ő勗��
    public float attacksPerSecond = 2; // �U���p�x
    public float maxSpinDuration = 3; // �ő��]����

    [Header("Bounce Sword Upgrade")]
    [SerializeField] private GameObject bounceSwordPrefab;
    [Range(0, 10)]
    [SerializeField] private float bounceThrowPower = 5;
    public int bounceCount = 5; // �o�E���X��
    public float bounceSpeed = 12; // �o�E���X���x

    [Header("Trajectory Prediction")]
    [SerializeField] private GameObject predictionDot; // �\���\���p�h�b�g
    [SerializeField] private int numberOfDots = 20;    // �h�b�g��
    [SerializeField] private float spaceBetweenDots = 0.05f; // �h�b�g�Ԋu
    private float swordGravity;  // ���ɂ�����d�̓X�P�[��
    private Transform[] dots;    // �O���\���h�b�g
    private Vector2 confirmedDirection; // �����m�����

    protected override void Awake()
    {
        base.Awake();
        swordGravity = swordPrefab.GetComponent<Rigidbody2D>().gravityScale; // ���̏d�̓X�P�[���擾
        dots = GenerateDots(); // �O���\���h�b�g����
    }

    // �X�L���g�p�\������
    public override bool CanUseSkill()
    {
        UpdateThrowPower(); // ���݂̃A�b�v�O���[�h�ɉ����������͍X�V

        if (currentSword != null)
        {
            currentSword.GetSwordBackToPlayer(); // ��������߂�
            return false;
        }

        return base.CanUseSkill();
    }

    // ���𓊂���
    public void ThrowSword()
    {
        GameObject swordPrefab = GetSwordPrefab();
        GameObject newSword = Instantiate(swordPrefab, dots[1].position, Quaternion.identity);

        currentSword = newSword.GetComponent<SkillObject_Sword>();
        currentSword.SetupSword(this, GetThrowPower());

        SetSkillOnCooldown(); // �g�p��ɃN�[���_�E���J�n
    }

    // ���݂̃A�b�v�O���[�h�ɉ��������v���n�u��Ԃ�
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

    // ���݂̃A�b�v�O���[�h�ɉ����ē����͂�ݒ�
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
                break;
        }
    }

    // �������x�x�N�g����v�Z
    private Vector2 GetThrowPower() => confirmedDirection * (currentThrowPower * 10);

    // �O���\���X�V
    public void PredictTrajectory(Vector2 direction)
    {
        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].position = GetTrajectoryPoint(direction, i * spaceBetweenDots);
        }
    }

    // ����t��̗\���ʒu��v�Z
    private Vector2 GetTrajectoryPoint(Vector2 direction, float t)
    {
        float scaledThrowPower = currentThrowPower * 10;
        Vector2 initialVelocity = direction * scaledThrowPower; // �����x�N�g��
        Vector2 gravityEffect = 0.5f * Physics2D.gravity * swordGravity * (t * t); // �d�͂ɂ��ʒu�ω�
        Vector2 predictedPoint = (initialVelocity * t) + gravityEffect; // �����ʒu
        Vector2 playerPosition = transform.root.position;

        return playerPosition + predictedPoint;
    }

    public void ConfirmTrajectory(Vector2 direction) => confirmedDirection = direction;

    // �h�b�g�̕\���ؑ�
    public void EnableDots(bool enable)
    {
        foreach (Transform t in dots)
            t.gameObject.SetActive(enable);
    }

    // �h�b�g����
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
