using System.Threading;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightUpPlayer : MonoBehaviour
{
    private Light2D lampLight;

    [Header("Light details")]
    [SerializeField] private Light2D playerLight; // 暗いマップ内でプレイヤーのライト参照を取得する

    [Header("Flickering details")]
    [SerializeField] private float flickerInterval; // 秒単位、ライトの強度を変化させてチラつきをシミュレートする間隔
    private float targetIntensity;
    private float baseIntensity = 1f;
    private float minFlicker = -0.5f;
    private float maxFlicker = 1f;
    private float changeSpeed = 5f;
    private float timer;

    private void Start()
    {
        lampLight = GetComponentInChildren<Light2D>();

        playerLight.intensity = 0.5f; // 暗いマップ内でのプレイヤーライトの基本強度
        targetIntensity = baseIntensity;
    }

    private void Update()
    {
        SetLightFlicker();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // プレイヤーがライトの下にいるときのみ影響する
            playerLight.intensity = 1.5f;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerLight.intensity = 0.5f;
    }

    private void SetLightFlicker()
    {
        if (timer >= flickerInterval) // ランダムなチラつき時間、古く使い込まれたランプをシミュレートする
        {
            targetIntensity = baseIntensity + Random.Range(minFlicker, maxFlicker); // チラつき速度の範囲
            flickerInterval = Random.Range(0.05f, 0.3f);
            timer = 0;
        }

        lampLight.intensity = Mathf.Lerp(lampLight.intensity, targetIntensity, Time.deltaTime * changeSpeed); // ライトの強度の範囲、明るさの増減
        timer += Time.deltaTime;
    }


}
