using System;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class UI_ConnectionDetails
{
    public UI_ConnectionsHandler childNode;
    public NodeDirectionType direction;
    [Range(0, 300f)] public float length;
}

[ExecuteAlways]
public class UI_ConnectionsHandler : MonoBehaviour
{
    private RectTransform rect => GetComponent<RectTransform>();
    [SerializeField] private UI_ConnectionDetails[] connectionDetails;
    [SerializeField] private UI_TreeConnections[] connections;

    private void OnValidate()
    {
        if (connectionDetails.Length <= 0)
            return;

        UpdateConnection();
    }

    private void UpdateConnection()
    {
        for (int i = 0; i < connectionDetails.Length; i++)
        {
            var detail = connectionDetails[i];
            var connection = connections[i];

            connection.DirectConnection(detail.direction, detail.length);

            Vector2 targetPosition = connection.GetConnectionPoint(rect);
            detail.childNode.SetPosition(targetPosition);
        }
    }

    public void SetPosition(Vector2 position) => rect.anchoredPosition = position;
}
