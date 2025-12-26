using Unity.InferenceEngine;

using UnityEngine;

public class SentisTest : MonoBehaviour
{
    public ModelAsset modelAsset;

    void Start()
    {
        var model = ModelLoader.Load(modelAsset);
        Debug.Log("✅ Sentis is working");
    }
}
