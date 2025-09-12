using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class UI_ConnectionDetails
{
    public UI_TreeConnectHandler childNode;   // 接続先の子ノード
    public NodeDirectionType direction;       // 接続方向
    [Range(0, 300f)] public float length;    // 接続線の長さ
    [Range(-50f, 50f)] public float rotation;// 接続線の回転
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

    // 子ノードをすべて取得
    public UI_TreeNode[] GetChildNodes()
    {
        List<UI_TreeNode> childrenToReturn = new List<UI_TreeNode>();
        foreach (var node in connectionDetails)
        {
            if (node.childNode != null)
                childrenToReturn.Add(node.childNode.GetComponent<UI_TreeNode>());
        }
        return childrenToReturn.ToArray();
    }

    // 接続線の更新
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

            // 子ノードの位置を接続線の先端に設定
            detail.childNode.SetPosition(targetPosition);
            detail.childNode?.SetConnectionImage(connectionImage);
        }
    }

    // 再帰的に全接続を更新
    public void UpdateAllConnections()
    {
        UpdateConnections();
        foreach (var node in connectionDetails)
        {
            if (node.childNode == null) continue;
            node.childNode?.UpdateConnections();
        }
    }

    // 接続線の色をアンロック状態に応じて変更
    public void UnlockConnectionImage(bool unlocked)
    {
        if (connectionImage == null) return;
        connectionImage.color = unlocked ? unlockedConnectionColor : originalColor;
    }

    public void SetConnectionImage(Image image) => connectionImage = image;

    public void SetPosition(Vector2 position) => rect.anchoredPosition = position;

    private void OnValidate()
    {
        if (connectionDetails.Length <= 0) return;

        if (connectionDetails.Length != connections.Length)
        {
            Debug.Log("Amount of details should be same as amount of connections. - " + gameObject.name);
            return;
        }

        UpdateConnections();
    }
}
