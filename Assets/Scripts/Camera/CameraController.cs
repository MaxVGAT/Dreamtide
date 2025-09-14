using UnityEngine.SceneManagement;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Entity_Player player;

    [Header("Camera details")]
    public float moveSpeed;
    public float lookAheadDistance = 2.5f;
    public float lookAheadSpeed = 0.5f;
    public float verticalOffset = 2f;
    private float lookOffset;
    public float maxVertOffset = 5f;
    private bool isFalling;

    [Header("Village bounds")]
    public bool useBounds = false; // only active in specific scenes
    public Vector2 minBounds;      // bottom-left corner
    public Vector2 maxBounds;      // top-right corner

    private Vector3 targetPoint = Vector3.zero;

    void Awake()
    {
        useBounds = (SceneManager.GetActiveScene().name == "Village");
    }

    private void Start()
    {
        targetPoint = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z); // �J�����̏����ʒu��v���C���[�̈ʒu�ɐݒ�
    }

    private void LateUpdate()
    {
        if (player.isGrounded)
            targetPoint.y = player.transform.position.y; // �v���C���[�̍����ɍ��킹�ăJ�����̍�������肳����

        float targetY = player.transform.position.y + verticalOffset;

        // �v���C���[�̈ʒu���ő�I�t�Z�b�g���Ⴂ�ꍇ�͗������Ƃ݂Ȃ��A�J�����������
        if (transform.position.y - (player.transform.position.y + verticalOffset) > maxVertOffset)
            isFalling = true;

        // �J������v���C���[��Y�ʒu�Ƀt�H�[�J�X������
        if (isFalling)
        {
            targetY = player.transform.position.y;
            if (player.isGrounded)
                isFalling = false;
        }

        // �v���C���[�̌����ɉ����ĉ�ʂ̃X�y�[�X��L�����p���邽�߂̃I�t�Z�b�g��ǉ�
        float targetLookOffset = lookAheadDistance * player.facingDirection;
        lookOffset = Mathf.Lerp(lookOffset, targetLookOffset, lookAheadSpeed * Time.deltaTime);

        // �����̏����g�ݍ��킹�āA�J������v���C���[�̑O���ɓ��I�ɔz�u����
        targetPoint = new Vector3(player.transform.position.x + lookOffset, targetY, transform.position.z);

        if (useBounds)
        {
            targetPoint.x = Mathf.Clamp(targetPoint.x, minBounds.x, maxBounds.x);
            targetPoint.y = Mathf.Clamp(targetPoint.y, minBounds.y, maxBounds.y);
        }

        // �J�����̓�����X���[�Y�ɂ��āA�}�ɓ����Ȃ��悤�ɂ���
        transform.position = Vector3.Lerp(transform.position, targetPoint, moveSpeed * Time.deltaTime);
    }
}
