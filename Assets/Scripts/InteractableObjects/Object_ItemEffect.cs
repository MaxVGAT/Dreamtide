using System.Collections;
using UnityEngine;

[System.Serializable]
public class Buff
{
    public StatType type;
    public float value;
}

public class Object_ItemEffect : MonoBehaviour
{
    private SpriteRenderer sr;
    private Entity_Stats statsToModify;

    [Header("Buff details")]
    [SerializeField] private Buff[] buffs;
    [SerializeField] private string buffName;
    [SerializeField] private float buffDuration = 4f;
    [SerializeField] private bool canBeUsed = true;

    [Header("Pulse details")]
    [SerializeField] private float pulseSpeed = 1;
    [SerializeField] private float minScale = 0.8f;
    [SerializeField] private float maxScale = 1.2f;
    [SerializeField] private float timeOffset = 0f;

    private Vector3 originalScale;

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        originalScale = this.transform.localScale;

        timeOffset = Random.Range(0f, Mathf.PI * 2);
    }

    private void Update()
    {
        float sineValue = Mathf.Sin((Time.time + timeOffset) * pulseSpeed);

        float pulseScale = Mathf.Lerp(minScale, maxScale, (sineValue + 1f) / 2f);

        this.transform.localScale = originalScale * pulseScale;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (canBeUsed == false)
            return;

        statsToModify = collision.GetComponent<Entity_Stats>();
        StartCoroutine(BuffCo(buffDuration));
    }

    private IEnumerator BuffCo(float duration)
    {
        canBeUsed = false;
        sr.color = Color.clear;

        ApplyBuff(true);

        yield return new WaitForSeconds(duration);

        ApplyBuff(false);
        Destroy(gameObject);
    }

    private void ApplyBuff(bool apply)
    {
        foreach (var buff in buffs)
        {
            if (apply)
                statsToModify.GetStatByType(buff.type).AddModifier(buff.value, buffName);
            else
                statsToModify.GetStatByType(buff.type).RemoveModifier(buffName);
        }
    }
}
