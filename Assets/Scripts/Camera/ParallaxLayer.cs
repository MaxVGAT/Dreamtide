using UnityEngine;

[System.Serializable]
public class ParallaxLayer
{
    [SerializeField] private Transform background;
    [SerializeField] private float parallaxMultiplier;

    private float imageFullWidth;
    private float imageHalfWidth;

    public void CalculateImageWidth()
    {
        // 画像の全幅を取得して半分の幅を計算するために必要
        imageFullWidth = background.GetComponent<SpriteRenderer>().bounds.size.x;
        // シームレスな背景ループで使うために半分の幅を計算する
        imageHalfWidth = imageFullWidth / 2;
    }

    // カメラの移動距離に応じて背景を動かし、大きな動きを避ける
    public void MoveParallaxImages(float distanceToMove)
    {
        background.position += Vector3.right * (distanceToMove * parallaxMultiplier);
    }

    // 3枚の画像（中央、左、右）だけで無限背景を作るために、範囲外になった画像の位置を入れ替える
    public void LoopBackground(float cameraLeftEdge, float cameraRightEdge)
    {
        float imageRightEdge = background.position.x + imageHalfWidth;
        float imageLeftEdge = background.position.x - imageHalfWidth;

        if (imageRightEdge < cameraLeftEdge)
            background.position += Vector3.right * imageFullWidth;
        else if (imageLeftEdge > cameraRightEdge)
            background.position += Vector3.right * -imageFullWidth;
    }
}
