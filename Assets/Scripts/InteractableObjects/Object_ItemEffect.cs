using System.Collections;
using UnityEngine;

public class Object_ItemEffect : MonoBehaviour
{
    private Player_Stats statsToModify;     // �o�t�Ώۂ̃X�e�[�^�X

    [Header("Buff details")]
    [SerializeField] private BuffEffectData[] buffs;        // �K�p����o�t�ꗗ
    [SerializeField] private string buffName;     // �o�t���i���ʗp�j
    [SerializeField] private float buffDuration = 4f; // �o�t�̎�������

    [Header("Pulse details")]
    [SerializeField] private float pulseSpeed = 1;    // �p���X���x
    [SerializeField] private float minScale = 0.8f;  // �ŏ��X�P�[��
    [SerializeField] private float maxScale = 1.2f;  // �ő�X�P�[��
    [SerializeField] private float timeOffset = 0f;  // �A�j���[�V�����ʑ��I�t�Z�b�g

    private Vector3 originalScale;  // ���̃X�P�[��

    private void Awake()
    {
        originalScale = this.transform.localScale;

        timeOffset = Random.Range(0f, Mathf.PI * 2); // �o�t���ƂɃA�j���[�V�������炷
    }

    private void Update()
    {
        // �p���X�A�j���[�V�����v�Z
        float sineValue = Mathf.Sin((Time.time + timeOffset) * pulseSpeed);
        float pulseScale = Mathf.Lerp(minScale, maxScale, (sineValue + 1f) / 2f);

        this.transform.localScale = originalScale * pulseScale;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        statsToModify = collision.GetComponent<Player_Stats>();

        if (statsToModify.CanApplyBuffOf(buffName))
        {
            statsToModify.ApplyBuff(buffs, buffDuration, buffName); // �o�t�K�p�J�n
            Destroy(gameObject);
        }
    }
}
