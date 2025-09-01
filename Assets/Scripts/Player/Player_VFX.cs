using System.Collections;
using UnityEngine;

public class Player_VFX : Entity_VFX
{
    [Header("Image Echo VFX")]
    [SerializeField, Range(0.01f, 0.2f)] private float imageEchoInterval = 0.05f; // エコー間隔
    [SerializeField] private GameObject imageEchoPrefab; // エコー用プレハブ
    private Coroutine imageEchoCo;

    // 指定時間だけイメージエコーを発生させる
    public void DoImageEchoEffect(float duration)
    {
        if (imageEchoCo != null)
            StopCoroutine(imageEchoCo);

        imageEchoCo = StartCoroutine(ImageEchoEffectCo(duration));
    }

    // エコー処理コルーチン
    private IEnumerator ImageEchoEffectCo(float duration)
    {
        float time = 0;

        while (time < duration)
        {
            CreateImageEcho(); // エコー生成

            yield return new WaitForSeconds(imageEchoInterval);
            time = time + imageEchoInterval;
        }
    }

    // 現在のスプライトをコピーしてエコー生成
    private void CreateImageEcho()
    {
        GameObject imageEcho = Instantiate(imageEchoPrefab, transform.position, transform.rotation);
        imageEcho.GetComponentInChildren<SpriteRenderer>().sprite = sr.sprite;
    }
}
