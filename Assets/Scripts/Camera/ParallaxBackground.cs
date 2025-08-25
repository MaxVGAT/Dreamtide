using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{

    private Camera mainCamera;
    private float lastCamPosX;
    private float cameraHalfWidth;

    [SerializeField] private ParallaxLayer[] backgroundLayers;

    private void Awake()
    {
        mainCamera = Camera.main;
        // カメラの半分の幅を取得して、無限に続く背景の錯覚を作る
        cameraHalfWidth = mainCamera.orthographicSize * mainCamera.aspect;
        // 各画像の幅を計算して、ピクセル単位で正確に動かし、シームレスにする
        CalculateImageLength();
    }

    private void Update()
    {
        float currentCameraPositionX = mainCamera.transform.position.x;
        // カメラが動いた距離を取得し、画像の移動量を計算するために使う
        float distanceToMove = currentCameraPositionX - lastCamPosX;
        lastCamPosX = currentCameraPositionX;

        // 左右両方で無限背景を可能にするためのカメラの端の位置
        float cameraLeftEdge = currentCameraPositionX - cameraHalfWidth;
        float cameraRightEdge = currentCameraPositionX + cameraHalfWidth;

        // 配列内の全レイヤーを取得し、パラメータに従って同時に動かす
        foreach (ParallaxLayer layer in backgroundLayers)
        {
            layer.MoveParallaxImages(distanceToMove);
            // 背景をループさせて画像が描画されなくなるのを防ぐ
            layer.LoopBackground(cameraLeftEdge, cameraRightEdge);
        }
    }


    private void CalculateImageLength()
    {
        foreach(ParallaxLayer layer in  backgroundLayers)
            layer.CalculateImageWidth();
    }
}
