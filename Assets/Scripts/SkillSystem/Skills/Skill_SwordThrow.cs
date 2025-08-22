using UnityEngine;

public class Skill_SwordThrow : Skill_Base
{
    private SkillObject_Sword currentSword;
    private float currentThrowPower;

    [Header("Regular Sword Upgrade")]
    [SerializeField] private GameObject swordPrefab;
    [Range(0, 10)]
    [SerializeField] private float regularThrowPower = 5;

    [Header("Pierce sword Upgrade")]
    [SerializeField] private GameObject pierceSwordPrefab;
    public int amountToPierce = 2;
    [Range(0, 10)]
    [SerializeField] private float pierceThrowPower = 5;

    [Header("Spin sword upgrade")]
    [SerializeField] private GameObject spinSwordPrefab;
    [Range(0, 10)]
    [SerializeField] private float spinThrowPower = 5;
    public int maxDistance = 5;
    public float attacksPerSecond = 2;
    public float maxSpinDuration = 3;

    [Header("Bounce Sword Upgrade")]
    [SerializeField] private GameObject bounceSwordPrefab;
    [Range(0, 10)]
    [SerializeField] private float bounceThrowPower = 5;
    public int bounceCount = 5;
    public float bounceSpeed = 12;

    [Header("Trajectory Prediction")]
    [SerializeField] private GameObject predictionDot;
    [SerializeField] private int numberOfDots = 20;
    [SerializeField] private float spaceBetweenDots = 0.05f;
    private float swordGravity;
    private Transform[] dots;
    private Vector2 confirmedDirection;

    protected override void Awake()
    {
        base.Awake();
        swordGravity = swordPrefab.GetComponent<Rigidbody2D>().gravityScale;
        dots = GenerateDots();
    }

    public override bool CanUseSkill()
    {
        UpdateThrowPower();

        if (currentSword != null)
        {
            currentSword.GetSwordBackToPlayer();
            return false;
        }

        return base.CanUseSkill();
    }

    public void ThrowSword()
    {
        GameObject swordPrefab = GetSwordPrefab();
        GameObject newSword = Instantiate(swordPrefab, dots[1].position, Quaternion.identity);

        currentSword = newSword.GetComponent<SkillObject_Sword>();
        currentSword.SetupSword(this, GetThrowPower());
    }

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

    private void UpdateThrowPower()
    {
        switch(upgradeType)
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

    private Vector2 GetThrowPower() => confirmedDirection * (currentThrowPower * 10);

    public void PredictTrajectory(Vector2 direction)
    {
        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].position = GetTrajectoryPoint(direction, i * spaceBetweenDots);
        }
    }

    private Vector2 GetTrajectoryPoint(Vector2 direction, float t)
    {
        float scaledThrowPower = currentThrowPower * 10;

        Vector2 initialVelocity = direction * scaledThrowPower; // Starting speed and direction of the throw

        Vector2 gravityEffect = 0.5f * Physics2D.gravity * swordGravity * (t * t); // Gravity pulls the sword down, dropping the direction with air time, and calculate how far the sword will fly after time 't'

        Vector2 predictedPoint = (initialVelocity * t) + gravityEffect; // Combine initial direction and gravity pull

        Vector2 playerPosition = transform.root.position;

        return playerPosition + predictedPoint;
    }

    public void ConfirmTrajectory(Vector2 direction) => confirmedDirection = direction;

    public void EnableDots(bool enable)
    {
        foreach (Transform t in dots)
            t.gameObject.SetActive(enable);
    }

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
