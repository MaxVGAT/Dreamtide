using System.Collections;
using System;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public event Action OnFlipped; // �G���e�B�e�B�����]�����Ƃ��ɔ��΂���C�x���g

    public Animator anim { get; private set; } // Animator�R���|�[�l���g��擾���A�ǂݎ��\�ɂ���

    public Rigidbody2D rb { get; private set; } // Rigidbody2D�R���|�[�l���g��擾���A�ǂݎ��\�ɂ���
     // Entity_Stats�R���|�[�l���g��擾���A�ǂݎ��\�ɂ���

    protected StateMachine stateMachine; // �X�e�[�g�}�V���̎Q�Ƃ�L���b�V��

    public int facingDirection { get; private set; } = 1; // �����Ă�������i1���E�����j��ǂݎ��\�ɂ��A�����l��E�����ɐݒ�
    private bool facingRight = true; // �����̓�d�`�F�b�N�p�t���O

    [Header("Collision detection")]
    [SerializeField] public LayerMask whatIsGround; // �n�ʔ���p���C���[�}�X�N
    [SerializeField] private float groundCheckDistance; // �n�ʔ���p���C�L���X�g�̒���
    [SerializeField] private float wallCheckDistance; // �ǔ���p���C�L���X�g�̒���
    [SerializeField] private Transform groundCheck; // �n�ʔ���̋N�_
    [SerializeField] private Transform upperWallCheck; // �ǔ���̏㑤�N�_
    [SerializeField] private Transform lowerWallCheck; // �ǔ���̉����N�_

    public bool isWallDetected { get; private set; } // �ǂ��߂��ɂ��邩�ǂ���
    public bool isGrounded { get; private set; } // �n�ʂɐڒn���Ă��邩�ǂ���

    public virtual bool isBlocking => false; // ���N���X�ŃI�[�o�[���C�h�\�Ȗh���Ԕ���

    // �m�b�N�o�b�N�p�ϐ�
    private bool isKnocked;
    private Coroutine knockbackCo;

    // ���ł���уX���[�_�E�������pCoroutine
    private Coroutine despawnCo;
    private Coroutine slowDownCo;

    // �p����ŏ�����ύX�\�ɂ��邽��protected virtual
    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        
        stateMachine = new StateMachine();
    }

    protected virtual void Start()
    {
        // ��̂܂܌p����Ŏg�p�\
    }

    protected virtual void Update()
    {
        HandleCollisionDetection();
        stateMachine.UpdateActiveState();
    }

    // �A�j���[�V�����C�x���g����Ă΂��֐�
    public void CurrentStateAnimationTrigger()
    {
        stateMachine.currentState.AnimationTrigger();
    }

    public virtual void EntityDeath()
    {
        // �p����Ŏ���
    }

    // �X���[�_�E��������~�߂�
    public virtual void StopSlowDown()
    {
        slowDownCo = null;
    }

    // �X���[�_�E����K�p�B���ɃX���[�_�E�����Ȃ�A�D��x�ɂ���ď㏑���ۂ𔻒f
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

    // ���S���̃t�F�[�h�A�E�g�����J�n
    public void DespawnOnDeath(float duration)
    {
        if (despawnCo != null)
            StopCoroutine(despawnCo);

        despawnCo = StartCoroutine(DespawnOnDeathCo(2f));
    }

    private IEnumerator DespawnOnDeathCo(float duration) // �t�F�[�h�A�E�g���Ă���Q�[���I�u�W�F�N�g�j��
    {
        float timer = 0f;

        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();

        Color[] originalColors = new Color[sprites.Length]; // RGB�͕ς����ɃA���t�@�̂ݑ��삷�邽�ߌ��̐F��ۑ�
        for (int i = 0; i < sprites.Length; i++)
            originalColors[i] = sprites[i].color;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float alphaFade = Mathf.Lerp(1f, 0f, timer / duration); // 1����0�փA���t�@�l����`���

            for (int i = 0; i < sprites.Length; i++)
            {
                Color baseColor = originalColors[i];

                baseColor.r = 1f;                    // �h���}�`�b�N���ʂ̂��ߐԐF�ɕω�
                baseColor.g = 0f;
                baseColor.b = 0f;
                baseColor.a = alphaFade;             // �A���t�@�l�Ƀt�F�[�h��K�p

                sprites[i].color = baseColor;        // �F��K�p
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

    // �m�b�N�o�b�N��K�p
    private IEnumerator KnockbackCo(Vector2 knockback, float duration)
    {
        isKnocked = true;
        rb.linearVelocity = knockback;

        yield return new WaitForSeconds(duration);

        rb.linearVelocity = Vector2.zero;
        isKnocked = false;
    }

    // �ړ����x�ƕ�����Z�b�g���A�K�v�Ȃ甽�]�����
    public void SetVelocity(float xVelocity, float yVelocity)
    {
        if (isKnocked) return;

        rb.linearVelocity = new Vector2(xVelocity, yVelocity);
        HandleFlip(xVelocity);
    }

    // �ړ������ɍ��킹�ăX�v���C�g�𔽓]
    public void HandleFlip(float xVelocity)
    {
        if (xVelocity > 0 && facingRight == false)
            FlipMethod();
        else if (xVelocity < 0 && facingRight == true)
            FlipMethod();
    }

    // �X�v���C�g�̔��]����
    public void FlipMethod()
    {
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;
        facingDirection = facingDirection * -1;

        OnFlipped?.Invoke();
    }

    // �n�ʂƕǔ���p�̃��C�L���X�g��������A�ڒn�E�ǔ��茋�ʂ�X�V
    private void HandleCollisionDetection()
    {
        // �n�ʂ̔���
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);

        // �ǂ͏㉺2�ӏ��Ŕ��肵�A�����ڐG���Ă��Ȃ��ƕǂƂ��Ĕ��肵�Ȃ��i�ǂ��犊�藎���鏈��������j
        if (lowerWallCheck != null)
        {
            isWallDetected = Physics2D.Raycast(upperWallCheck.position, Vector2.right * facingDirection, wallCheckDistance, whatIsGround)
                          && Physics2D.Raycast(lowerWallCheck.position, Vector2.right * facingDirection, wallCheckDistance, whatIsGround);
        }
        else
            isWallDetected = Physics2D.Raycast(upperWallCheck.position, Vector2.right * facingDirection, wallCheckDistance, whatIsGround);
    }

    // Gizmos��g���ă��C�L���X�g�͈̔͂�����i�G�f�B�^��̂݁j
    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + new Vector3(0, -groundCheckDistance));
        Gizmos.DrawLine(upperWallCheck.position, upperWallCheck.position + new Vector3(wallCheckDistance * facingDirection, 0));

        if (lowerWallCheck != null)
            Gizmos.DrawLine(lowerWallCheck.position, lowerWallCheck.position + new Vector3(wallCheckDistance * facingDirection, 0));
    }
}
