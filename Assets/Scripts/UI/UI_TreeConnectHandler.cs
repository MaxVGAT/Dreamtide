using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class UI_ConnectionDetails
{
    public UI_TreeConnectHandler childNode;
    public NodeDirectionType direction;
    [Range(0, 300f)] public float length;
    [Range(-50f, 50f)] public float rotation;
}

[ExecuteAlways]
public class UI_TreeConnectHandler : MonoBehaviour
{

    private RectTransform rect => GetComponent<RectTransform>();
    [SerializeField] private UI_ConnectionDetails[] connectionDetails;
    [SerializeField] private UI_TreeConnections[] connections;

    private Image connectionImage;
    private Color originalColor;
    private Color unlockedConnectionColor;

    private void Awake()
    {
        if (connectionImage != null)
            originalColor = connectionImage.color;

        ColorUtility.TryParseHtmlString("#F6A765", out unlockedConnectionColor);
    }

    private void OnValidate()
    {
        if (connectionDetails.Length <= 0)
            return;

        UpdateConnections();
    }

    public void UpdateConnections()
    {
        for (int i = 0; i < connectionDetails.Length; i++)
        {
            var detail = connectionDetails[i];
            var connection = connections[i];

            connection.DirectConnection(detail.direction, detail.length, detail.rotation);
            Image connectionImage = connection.GetConnectionImage();

            Vector2 targetPosition = connection.GetConnectionPoint(rect);

            if (detail.childNode == null)
                continue;

            detail.childNode.SetPosition(targetPosition);
            detail.childNode?.SetConnectionImage(connectionImage);
            detail.childNode.transform.SetAsLastSibling();
        }
    }

    public void UpdateAllConnections()
    {
        UpdateConnections();

        foreach (var node in connectionDetails)
        {
            if (node.childNode == null) continue;
            node.childNode?.UpdateConnections();
        }
    }

    public void UnlockConnectionImage(bool unlocked)
    {
        if (connectionImage == null)
            return;

        connectionImage.color = unlocked ? unlockedConnectionColor : originalColor;
    }

    public void SetConnectionImage(Image image) => connectionImage = image;

    public void SetPosition(Vector2 position) => rect.anchoredPosition = position;
}
