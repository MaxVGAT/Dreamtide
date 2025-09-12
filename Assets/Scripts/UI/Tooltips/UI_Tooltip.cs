using UnityEngine;

// �c�[���`�b�v�\���p�N���X
public class UI_Tooltip : MonoBehaviour
{
    protected RectTransform rect;
    [SerializeField] private Vector2 offset = new Vector2(300, 20); // �c�[���`�b�v�̕\���I�t�Z�b�g

    protected virtual void Awake()
    {
        rect = GetComponent<RectTransform>();
        // �����ʒu���ʊO�ɐݒ�
        rect.position = new Vector2(9999, 9999);
    }

    // �c�[���`�b�v�\��/��\��
    public virtual void ShowToolTip(bool show, RectTransform targetRect)
    {
        if (rect == null)
            return;

        if (!show)
        {
            rect.position = new Vector2(9999, 9999); // ��\�����͉�ʊO��
            return;
        }

        UpdatePosition(targetRect);
    }

    // �c�[���`�b�v�̈ʒu�X�V
    private void UpdatePosition(RectTransform targetRect)
    {
        float screenCenterX = Screen.width / 2f;
        float screenTop = Screen.height;
        float screenBottom = 0;

        Vector2 targetPosition = targetRect.position;

        // ��ʂ̍��E�ŃI�t�Z�b�g���]
        targetPosition.x = targetPosition.x > screenCenterX
            ? targetPosition.x - offset.x
            : targetPosition.x + offset.x;

        float verticalHalf = rect.sizeDelta.y / 2f;
        float topY = targetPosition.y + verticalHalf;
        float bottomY = targetPosition.y - verticalHalf;

        // ��ʏ�[/���[�ɂ͂ݏo���Ȃ��悤�ɕ␳
        if (topY > screenTop)
            targetPosition.y = screenTop - verticalHalf - offset.y;
        else if (bottomY < screenBottom)
            targetPosition.y = screenBottom + verticalHalf + offset.y;

        rect.position = targetPosition;
    }

    // ������ɐF�t��
    protected string GetColoredText(string color, string text)
    {
        return $"<color={color}>{text}</color>";
    }
}
