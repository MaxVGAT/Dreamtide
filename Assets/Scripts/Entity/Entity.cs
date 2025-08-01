using System.Collections;
using System;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public event Action OnFlipped;

    public Animator anim { get; private set; }

    public Rigidbody2D rb { get; private set; }

    protected StateMachine stateMachine;

    public int facingDirection { get; private set; } = 1;
    private bool facingRight = true;

    [Header("Collision detection")]
    [SerializeField] protected LayerMask whatIsGround;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform upperWallCheck;
    [SerializeField] private Transform lowerWallCheck;

    public bool isWallDetected { get; private set; }
    public bool isGrounded { get; private set; }

    public virtual bool isBlocking => false;

    private bool isKnocked;
    private Coroutine knockbackCo;

    private Coroutine despawnCo;

    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();

        stateMachine = new StateMachine();
    }

    protected virtual void Start()
    {
       
    }

    protected virtual void Update()
    {
        HandleCollisionDetection();
        stateMachine.UpdateActiveState();
    }

    public void CurrentStateAnimationTrigger()
    {
        stateMachine.currentState.AnimationTrigger();
    }

    public virtual void EntityDeath()
    {

    }

    public void DespawnOnDeath(float duration)
    {
        if (despawnCo != null)
            StopCoroutine(despawnCo);

        despawnCo = StartCoroutine(DespawnOnDeathCo(2f));
    }

    private IEnumerator DespawnOnDeathCo(float duration) // Destroy the game object when dead with a fade out
    {
        float timer = 0f;

        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();

        Color[] originalColors = new Color[sprites.Length]; // Save the colors to change alpha without changing RGB
        for (int i = 0; i < sprites.Length; i++)
            originalColors[i] = sprites[i].color;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float alphaFade = Mathf.Lerp(1f, 0f, timer / duration); // From visible to fully invisible before destroying

            for (int i = 0; i < sprites.Length; i++)
            {
                Color baseColor = originalColors[i]; //Save base color

                baseColor.r = 1f;
                baseColor.g = 0f;
                baseColor.b = 0f;
                baseColor.a = alphaFade;             // Apply fade lerp to alpha

                sprites[i].color = baseColor;        //Apply new color
            }

            yield return null;
        }

        Destroy(gameObject);
    }

    public void ReceiveKnockback(Vector2 knockback, float duration)
    {
        if(knockbackCo !=null)
            StopCoroutine(knockbackCo);

        knockbackCo = StartCoroutine(KnockbackCo(knockback, duration));
    }

    private IEnumerator KnockbackCo(Vector2 knockback, float duration)
    {
        isKnocked = true;
        rb.linearVelocity = knockback;

        yield return new WaitForSeconds(duration);

        rb.linearVelocity = Vector2.zero;
        isKnocked = false;
    }
    public void SetVelocity(float xVelocity, float yVelocity)
    {

        if (isKnocked) return;

        rb.linearVelocity = new Vector2(xVelocity, yVelocity);
        HandleFlip(xVelocity);
    }

    public void HandleFlip(float xVelocity)
    {
        if (xVelocity > 0 && facingRight == false)
            FlipMethod();
        else if (xVelocity < 0 && facingRight == true)
            FlipMethod();
    }

    public void FlipMethod()
    {
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;
        facingDirection = facingDirection * -1;

        OnFlipped.Invoke();
    }

    private void HandleCollisionDetection()
    {
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);

        if (lowerWallCheck != null)
        {
            isWallDetected = Physics2D.Raycast(upperWallCheck.position, Vector2.right * facingDirection, wallCheckDistance, whatIsGround)
                          && Physics2D.Raycast(lowerWallCheck.position, Vector2.right * facingDirection, wallCheckDistance, whatIsGround);
        }
        else
            isWallDetected = Physics2D.Raycast(upperWallCheck.position, Vector2.right * facingDirection, wallCheckDistance, whatIsGround);
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + new Vector3(0, -groundCheckDistance));
        Gizmos.DrawLine(upperWallCheck.position, upperWallCheck.position + new Vector3(wallCheckDistance * facingDirection, 0));

        if(lowerWallCheck != null)
        Gizmos.DrawLine(lowerWallCheck.position, lowerWallCheck.position + new Vector3(wallCheckDistance * facingDirection, 0));
    }
}
