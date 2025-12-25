using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections;

public class CameraCapture : MonoBehaviour
{
    ARCameraManager cameraManager;

    void Awake()
    {
        cameraManager = GetComponent<ARCameraManager>();
    }

    public Texture2D CaptureFrame()
    {
        if (!cameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
            return null;

        var format = TextureFormat.RGBA32;
        Texture2D tex = new Texture2D(image.width, image.height, format, false);

        var conversionParams = new XRCpuImage.ConversionParams
        {
            inputRect = new RectInt(0, 0, image.width, image.height),
            outputDimensions = new Vector2Int(image.width, image.height),
            outputFormat = format,
            transformation = XRCpuImage.Transformation.MirrorY
        };

        image.Convert(conversionParams, tex.GetRawTextureData<byte>());
        tex.Apply();
        image.Dispose();

        return tex;
    }
}
