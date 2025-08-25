using System.Collections;
using System;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public event Action OnFlipped; // エンティティが反転したときに発火するイベント

    public Animator anim { get; private set; } // Animatorコンポーネントを取得し、読み取り可能にする

    public Rigidbody2D rb { get; private set; } // Rigidbody2Dコンポーネントを取得し、読み取り可能にする
    public Entity_Stats stats { get; private set; } // Entity_Statsコンポーネントを取得し、読み取り可能にする

    protected StateMachine stateMachine; // ステートマシンの参照をキャッシュ

    public int facingDirection { get; private set; } = 1; // 向いている方向（1＝右向き）を読み取り可能にし、初期値を右向きに設定
    private bool facingRight = true; // 方向の二重チェック用フラグ

    [Header("Collision detection")]
    [SerializeField] public LayerMask whatIsGround; // 地面判定用レイヤーマスク
    [SerializeField] private float groundCheckDistance; // 地面判定用レイキャストの長さ
    [SerializeField] private float wallCheckDistance; // 壁判定用レイキャストの長さ
    [SerializeField] private Transform groundCheck; // 地面判定の起点
    [SerializeField] private Transform upperWallCheck; // 壁判定の上側起点
    [SerializeField] private Transform lowerWallCheck; // 壁判定の下側起点

    public bool isWallDetected { get; private set; } // 壁が近くにあるかどうか
    public bool isGrounded { get; private set; } // 地面に接地しているかどうか

    public virtual bool isBlocking => false; // 他クラスでオーバーライド可能な防御状態判定

    // ノックバック用変数
    private bool isKnocked;
    private Coroutine knockbackCo;

    // 消滅およびスローダウン処理用Coroutine
    private Coroutine despawnCo;
    private Coroutine slowDownCo;

    // 継承先で処理を変更可能にするためprotected virtual
    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<Entity_Stats>();

        stateMachine = new StateMachine();
    }

    protected virtual void Start()
    {
        // 空のまま継承先で使用可能
    }

    protected virtual void Update()
    {
        HandleCollisionDetection();
        stateMachine.UpdateActiveState();
    }

    // アニメーションイベントから呼ばれる関数
    public void CurrentStateAnimationTrigger()
    {
        stateMachine.currentState.AnimationTrigger();
    }

    public virtual void EntityDeath()
    {
        // 継承先で実装
    }

    // スローダウン処理を止める
    public virtual void StopSlowDown()
    {
        slowDownCo = null;
    }

    // スローダウンを適用。既にスローダウン中なら、優先度によって上書き可否を判断
    public virtual void SlowDownEntityBy(float duration, float slowMultiplier, bool canOverrideSlowEffect = false)
    {
        if (slowDownCo != null)
        {
            if (canOverrideSlowEffect)
                StopCoroutine(slowDownCo);
            else
                return;
        }

        slowDownCo = StartCoroutine(SlowDownEntityCo(duration, slowMultiplier));
    }

    protected virtual IEnumerator SlowDownEntityCo(float duration, float slowMultiplier)
    {
        yield return null;
    }

    // 死亡時のフェードアウト処理開始
    public void DespawnOnDeath(float duration)
    {
        if (despawnCo != null)
            StopCoroutine(despawnCo);

        despawnCo = StartCoroutine(DespawnOnDeathCo(2f));
    }

    private IEnumerator DespawnOnDeathCo(float duration) // フェードアウトしてからゲームオブジェクト破棄
    {
        float timer = 0f;

        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();

        Color[] originalColors = new Color[sprites.Length]; // RGBは変えずにアルファのみ操作するため元の色を保存
        for (int i = 0; i < sprites.Length; i++)
            originalColors[i] = sprites[i].color;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float alphaFade = Mathf.Lerp(1f, 0f, timer / duration); // 1から0へアルファ値を線形補間

            for (int i = 0; i < sprites.Length; i++)
            {
                Color baseColor = originalColors[i];

                baseColor.r = 1f;                    // ドラマチック効果のため赤色に変化
                baseColor.g = 0f;
                baseColor.b = 0f;
                baseColor.a = alphaFade;             // アルファ値にフェードを適用

                sprites[i].color = baseColor;        // 色を適用
            }

            yield return null;
        }

        Destroy(gameObject);
    }

    public void ReceiveKnockback(Vector2 knockback, float duration)
    {
        if (knockbackCo != null)
            StopCoroutine(knockbackCo);

        knockbackCo = StartCoroutine(KnockbackCo(knockback, duration));
    }

    // ノックバックを適用
    private IEnumerator KnockbackCo(Vector2 knockback, float duration)
    {
        isKnocked = true;
        rb.linearVelocity = knockback;

        yield return new WaitForSeconds(duration);

        rb.linearVelocity = Vector2.zero;
        isKnocked = false;
    }

    // 移動速度と方向をセットし、必要なら反転を処理
    public void SetVelocity(float xVelocity, float yVelocity)
    {
        if (isKnocked) return;

        rb.linearVelocity = new Vector2(xVelocity, yVelocity);
        HandleFlip(xVelocity);
    }

    // 移動方向に合わせてスプライトを反転
    public void HandleFlip(float xVelocity)
    {
        if (xVelocity > 0 && facingRight == false)
            FlipMethod();
        else if (xVelocity < 0 && facingRight == true)
            FlipMethod();
    }

    // スプライトの反転処理
    public void FlipMethod()
    {
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;
        facingDirection = facingDirection * -1;

        OnFlipped.Invoke();
    }

    // 地面と壁判定用のレイキャストを処理し、接地・壁判定結果を更新
    private void HandleCollisionDetection()
    {
        // 地面の判定
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);

        // 壁は上下2箇所で判定し、両方接触していないと壁として判定しない（壁から滑り落ちる処理を実現）
        if (lowerWallCheck != null)
        {
            isWallDetected = Physics2D.Raycast(upperWallCheck.position, Vector2.right * facingDirection, wallCheckDistance, whatIsGround)
                          && Physics2D.Raycast(lowerWallCheck.position, Vector2.right * facingDirection, wallCheckDistance, whatIsGround);
        }
        else
            isWallDetected = Physics2D.Raycast(upperWallCheck.position, Vector2.right * facingDirection, wallCheckDistance, whatIsGround);
    }

    // Gizmosを使ってレイキャストの範囲を可視化（エディタ内のみ）
    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + new Vector3(0, -groundCheckDistance));
        Gizmos.DrawLine(upperWallCheck.position, upperWallCheck.position + new Vector3(wallCheckDistance * facingDirection, 0));

        if (lowerWallCheck != null)
            Gizmos.DrawLine(lowerWallCheck.position, lowerWallCheck.position + new Vector3(wallCheckDistance * facingDirection, 0));
    }
}
