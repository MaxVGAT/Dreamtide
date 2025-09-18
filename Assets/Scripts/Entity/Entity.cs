using System.Collections;
using System;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public event Action OnFlipped; // 向き反転時のイベント

    public Animator anim { get; private set; } // アニメーター取得
    public Rigidbody2D rb { get; private set; } // Rigidbody2D取得
    public Entity_SFX sfx { get; private set; } // サウンド管理コンポーネント

    protected StateMachine stateMachine; // 状態管理

    public int facingDirection { get; private set; } = 1; // 向き（1=右、-1=左）
    private bool facingRight = true; // 右向きかどうか

    [Header("Collision detection")]
    [SerializeField] public LayerMask whatIsGround; // 地面レイヤー
    [SerializeField] private float groundCheckDistance; // 地面判定距離
    [SerializeField] private float wallCheckDistance; // 壁判定距離
    [SerializeField] private Transform groundCheck; // 地面判定位置
    [SerializeField] private Transform upperWallCheck; // 上壁判定位置
    [SerializeField] private Transform lowerWallCheck; // 下壁判定位置（任意）

    public bool isWallDetected { get; private set; } // 壁検出状態
    public bool isGrounded { get; private set; } // 地面接地状態

    public virtual bool isBlocking => false; // 防御状態（継承で上書き可能）

    private bool isKnocked; // ノックバック中か
    private Coroutine knockbackCo; // ノックバックCoroutine

    private Coroutine despawnCo; // 消滅Coroutine
    private Coroutine slowDownCo; // スローダウンCoroutine

    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sfx = GetComponent<Entity_SFX>();

        stateMachine = new StateMachine(); // 状態機初期化
    }

    protected virtual void Start()
    {
        // 初期処理
    }

    protected virtual void Update()
    {
        HandleCollisionDetection(); // 接地・壁判定
        stateMachine.UpdateActiveState(); // 現在の状態更新
    }

    public void CurrentStateAnimationTrigger()
    {
        stateMachine.currentState.AnimationTrigger(); // 現在状態のアニメーション呼び出し
    }

    public virtual void EntityDeath()
    {
        // 死亡処理（継承で実装）
    }

    public virtual void StopSlowDown()
    {
        slowDownCo = null; // スローダウン解除
    }

    public virtual void SlowDownEntityBy(float duration, float slowMultiplier, bool canOverrideSlowEffect = false)
    {
        if (slowDownCo != null)
        {
            if (canOverrideSlowEffect)
                StopCoroutine(slowDownCo);
            else
                return;
        }

        slowDownCo = StartCoroutine(SlowDownEntityCo(duration, slowMultiplier)); // スローダウン開始
    }

    protected virtual IEnumerator SlowDownEntityCo(float duration, float slowMultiplier)
    {
        yield return null; // 継承先で処理
    }

    public void DespawnOnDeath(float duration)
    {
        if (despawnCo != null)
            StopCoroutine(despawnCo);

        despawnCo = StartCoroutine(DespawnOnDeathCo(2f)); // 消滅開始
    }

    private IEnumerator DespawnOnDeathCo(float duration)
    {
        float timer = 0f;

        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
        Color[] originalColors = new Color[sprites.Length]; // 元色保存
        for (int i = 0; i < sprites.Length; i++)
            originalColors[i] = sprites[i].color;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float alphaFade = Mathf.Lerp(1f, 0f, timer / duration); // フェード計算

            for (int i = 0; i < sprites.Length; i++)
            {
                Color baseColor = originalColors[i];

                baseColor.r = 1f; // 赤色フェード
                baseColor.g = 0f;
                baseColor.b = 0f;
                baseColor.a = alphaFade;

                sprites[i].color = baseColor;
            }

            yield return null;
        }

        Destroy(gameObject); // 完全消滅
    }

    public void ReceiveKnockback(Vector2 knockback, float duration)
    {
        if (knockbackCo != null)
            StopCoroutine(knockbackCo);

        knockbackCo = StartCoroutine(KnockbackCo(knockback, duration)); // ノックバック適用
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
        if (isKnocked) return; // ノックバック中は無効

        rb.linearVelocity = new Vector2(xVelocity, yVelocity);
        HandleFlip(xVelocity); // 向き反転判定
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
        facingDirection *= -1;

        OnFlipped?.Invoke(); // 向き反転イベント発火
    }

    private void HandleCollisionDetection()
    {
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround); // 接地判定

        if (lowerWallCheck != null)
        {
            // 上下両方で壁判定
            isWallDetected = Physics2D.Raycast(upperWallCheck.position, Vector2.right * facingDirection, wallCheckDistance, whatIsGround)
                          && Physics2D.Raycast(lowerWallCheck.position, Vector2.right * facingDirection, wallCheckDistance, whatIsGround);
        }
        else
            isWallDetected = Physics2D.Raycast(upperWallCheck.position, Vector2.right * facingDirection, wallCheckDistance, whatIsGround); // 上のみ判定
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + new Vector3(0, -groundCheckDistance)); // 接地線
        Gizmos.DrawLine(upperWallCheck.position, upperWallCheck.position + new Vector3(wallCheckDistance * facingDirection, 0)); // 上壁線

        if (lowerWallCheck != null)
            Gizmos.DrawLine(lowerWallCheck.position, lowerWallCheck.position + new Vector3(wallCheckDistance * facingDirection, 0)); // 下壁線
    }
}
