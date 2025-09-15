using UnityEngine;
using UnityEngine.EventSystems;

// �{�^���z�o�[���Ɋg��E�k���̃p���X�A�j���[�V������SFX��Đ�
public class ButtonHoverPulse : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [Header("Hover Settings")]
    [SerializeField] private Vector3 hoverScale = new Vector3(1.1f, 1.1f, 1.1f); // �z�o�[���̊g��T�C�Y
    [SerializeField] private float pulseSpeed = 1f; // �p���X���x

    private Vector3 originalScale; // ���̃T�C�Y
    private bool isHovering = false; // �z�o�[�����ǂ���

    private void Start()
    {
        originalScale = transform.localScale; // �����T�C�Y�擾
    }

    private void Update()
    {
        // �z�o�[���Ȃ�g��A�����łȂ���Ό��̃T�C�Y�ɕ��
        Vector3 targetScale = isHovering ? Vector3.Scale(originalScale, hoverScale) : originalScale;
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * pulseSpeed);
    }

    // �}�E�X���{�^����ɓ������Ƃ�
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        EventSystem.current.SetSelectedGameObject(gameObject); // �I���Ԃɐݒ�
    }

    // �}�E�X���{�^���ォ��o���Ƃ�
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }

    // UI�I����i�L�[�{�[�h��R���g���[���[����j
    public void OnSelect(BaseEventData eventData)
    {
        isHovering = true;
    }

    // UI�I������
    public void OnDeselect(BaseEventData eventData)
    {
        isHovering = false;

        // �I�������Ƀ|�C���^�[�ޏo�C�x���g�𑗐M
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        ExecuteEvents.Execute(gameObject, pointerData, ExecuteEvents.pointerExitHandler);
    }
}
