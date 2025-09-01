using System.Collections;
using UnityEngine;

// 自動VFX制御：位置・回転のランダム化、フェード、オブジェクト破棄など
public class VFX_AutoController : MonoBehaviour
{
    private SpriteRenderer sr; // 表示用スプライト

    [SerializeField] private bool autoDestroy = true; // 自動破棄フラグ
    [SerializeField] private float destroyDelay = 1; // 破棄までの時間

    [Space]
    [SerializeField] private bool randomOffset = true; // 位置をランダム化するか
    [SerializeField] private bool randomRotation = true; // 回転をランダム化するか

    [Header("Fade effect")]
    [SerializeField] private float fadeSpeed = 1; // フェード速度
    [SerializeField] private bool canFade; // フェードを有効にするか

    [Header("Random Rotation")]
    [SerializeField] private float minRotation = 0;
    [SerializeField] private float maxRotation = 360;

    [Header("Random Position")]
    [SerializeField] private float xMinOffset = -0.3f;
    [SerializeField] private float xMaxOffset = 0.3f;
    [Space]
    [SerializeField] private float yMinOffset = -0.3f;
    [SerializeField] private float yMaxOffset = 0.3f;

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>(); // スプライト取得
    }

    private void Start()
    {
        if (canFade)
            StartCoroutine(FadeCo()); // フェード開始

        ApplyRandomOffset();   // 位置ランダム化
        ApplyRandomRotation(); // 回転ランダム化

        if (autoDestroy)
            Destroy(gameObject, destroyDelay); // 指定時間後に破棄
    }

    // フェードコルーチン
    private IEnumerator FadeCo()
    {
        Color targetColor = Color.white;

        while (targetColor.a > 0)
        {
            targetColor.a -= fadeSpeed * Time.deltaTime;
            sr.color = targetColor;
            yield return null;
        }

        sr.color = targetColor;
    }

    // 位置ランダム化
    private void ApplyRandomOffset()
    {
        if (!randomOffset)
            return;

        float xOffset = Random.Range(xMinOffset, xMaxOffset);
        float yOffset = Random.Range(yMinOffset, yMaxOffset);

        transform.position += new Vector3(xOffset, yOffset);
    }

    // 回転ランダム化
    private void ApplyRandomRotation()
    {
        if (!randomRotation)
            return;

        float zRotation = Random.Range(minRotation, maxRotation);
        transform.Rotate(0, 0, zRotation);
    }
}
