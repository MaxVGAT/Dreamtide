using UnityEngine;

public class Skill_SwordThrow : Skill_Base
{
    [Header("Sword Details")]
    [SerializeField] private float throwPower = 5;
    [SerializeField] private float swordGravity = 3.5f;

    [Header("Trajectory Prediction")]
    [SerializeField] private GameObject predictionDot;
    [SerializeField] private int numberOfDots = 20;
    [SerializeField] private float spaceBetweenDots = 0.05f;
    private Transform[] dots;
    private Vector2 confirmedDirection;

    protected override void Awake()
    {
        base.Awake();
        dots = GenerateDots();
    }

    public void ThrowSword()
    {
        Debug.Log("Create new sword");
    }

    public void PredictTrajectory(Vector2 direction)
    {
        for(int i = 0; i < dots.Length; i++)
        {
            dots[i].position = GetTrajectoryPoint(direction, i * spaceBetweenDots);
        }
    }

    private Vector2 GetTrajectoryPoint(Vector2 direction, float t)
    {
        float scaledThrowPower = throwPower * 10;

        Vector2 initialVelocity = direction * scaledThrowPower; // Starting speed and direction of the throw

        Vector2 gravityEffect = 0.5f * Physics2D.gravity * swordGravity * (t * t); // Gravity pulls the sword down, dropping the direction with air time, and calculate how far the sword will fly after time 't'

        Vector2 predictedPoint = (initialVelocity * t) + gravityEffect; // Combine initial direction and gravity pull

        Vector2 playerPosition = transform.root.position;

        return playerPosition + predictedPoint;
    }

    public void ConfirmTrajectory(Vector2 direction) => confirmedDirection = direction;

    public void EnableDots(bool enable)
    {
        foreach(Transform t in dots)
            t.gameObject.SetActive(enable);
    }

    private Transform[] GenerateDots()
    {
        Transform[] newDots = new Transform[numberOfDots];

        for(int i =0; i < numberOfDots; i++)
        {
            newDots[i] = Instantiate(predictionDot, transform.position, Quaternion.identity, transform).transform;
            newDots[i].gameObject.SetActive(false);
        }

        return newDots;
    }
}
