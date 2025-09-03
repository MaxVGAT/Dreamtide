using System.Collections;
using UnityEngine;

public class Player_VFX : Entity_VFX
{
    [Header("Image Echo VFX")]
    [SerializeField, Range(0.01f, 0.2f)] private float imageEchoInterval = 0.05f; // �G�R�[�Ԋu
    [SerializeField] private GameObject imageEchoPrefab; // �G�R�[�p�v���n�u
    private Coroutine imageEchoCo;

    public void CreateEffectOf(GameObject effect, Transform target)
    {
        Instantiate(effect, target.position, Quaternion.identity);
    }

    // �w�莞�Ԃ����C���[�W�G�R�[�𔭐�������
    public void DoImageEchoEffect(float duration)
    {
        if (imageEchoCo != null)
            StopCoroutine(imageEchoCo);

        imageEchoCo = StartCoroutine(ImageEchoEffectCo(duration));
    }

    // �G�R�[�����R���[�`��
    private IEnumerator ImageEchoEffectCo(float duration)
    {
        float time = 0;

        while (time < duration)
        {
            CreateImageEcho(); // �G�R�[����

            yield return new WaitForSeconds(imageEchoInterval);
            time = time + imageEchoInterval;
        }
    }

    // ���݂̃X�v���C�g��R�s�[���ăG�R�[����
    private void CreateImageEcho()
    {
        GameObject imageEcho = Instantiate(imageEchoPrefab, transform.position, transform.rotation);
        imageEcho.GetComponentInChildren<SpriteRenderer>().sprite = sr.sprite;
    }
}
