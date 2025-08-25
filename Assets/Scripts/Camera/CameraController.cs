using Unity.VisualScripting;
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

    private Vector3 targetPoint = Vector3.zero;

    private void Start()
    {
        targetPoint = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z); // カメラの初期位置をプレイヤーの位置に設定
    }

    private void LateUpdate()
    {
        if (player.isGrounded)
            targetPoint.y = player.transform.position.y; // プレイヤーの高さに合わせてカメラの高さを安定させる

        float targetY = player.transform.position.y + verticalOffset;

        // プレイヤーの位置が最大オフセットより低い場合は落下中とみなし、カメラを下げる
        if (transform.position.y - (player.transform.position.y + verticalOffset) > maxVertOffset)
            isFalling = true;

        // カメラをプレイヤーのY位置にフォーカスさせる
        if (isFalling)
        {
            targetY = player.transform.position.y;
            if (player.isGrounded)
                isFalling = false;
        }

        // プレイヤーの向きに応じて画面のスペースを有効活用するためのオフセットを追加
        float targetLookOffset = lookAheadDistance * player.facingDirection;
        lookOffset = Mathf.Lerp(lookOffset, targetLookOffset, lookAheadSpeed * Time.deltaTime);

        // これらの条件を組み合わせて、カメラをプレイヤーの前方に動的に配置する
        targetPoint = new Vector3(player.transform.position.x + lookOffset, targetY, transform.position.z);

        // カメラの動きをスムーズにして、急に動かないようにする
        transform.position = Vector3.Lerp(transform.position, targetPoint, moveSpeed * Time.deltaTime);
    }
}
