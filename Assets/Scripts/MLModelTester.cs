using UnityEngine;

#if UNITY_BARRACUDA
using Unity.Barracuda;
#endif

/// <summary>
/// Quick tester for ML model
/// Attach to any GameObject and press Play to test model loading
/// </summary>
public class MLModelTester : MonoBehaviour
{
    void Start()
    {
        TestModelLoading();
        TestInference();
    }

    void TestModelLoading()
    {
        Debug.Log("\n" + new string('=', 50));
        Debug.Log("Testing Model Loading...");
        Debug.Log(new string('=', 50));

#if UNITY_BARRACUDA
        try
        {
            // Try to load the model
            var model = Resources.Load<Model>("Models/waste_classifier");
            
            if (model != null)
            {
                Debug.Log("✓ SUCCESS: Model loaded via Resources");
                Debug.Log($"  Model name: {model.name}");
                Debug.Log($"  Inputs: {string.Join(", ", model.inputs.ToArray())}");
                Debug.Log($"  Outputs: {string.Join(", ", model.outputs.ToArray())}");
            }
            else
            {
                Debug.LogError("✗ FAILED: Model not found in Resources");
                Debug.LogError("  Expected path: Assets/Models/waste_classifier.onnx");
                Debug.LogError("  Make sure it's imported as a Neural Network Model");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"✗ ERROR: {e.Message}");
        }
#else
        Debug.LogError("✗ Barracuda is not installed!");
        Debug.LogError("  Install via Package Manager: com.unity.barracuda");
#endif
    }

    void TestInference()
    {
        Debug.Log("\n" + new string('=', 50));
        Debug.Log("Testing Inference...");
        Debug.Log(new string('=', 50));

        BarracudaMLModel mlModel = GetComponent<BarracudaMLModel>();
        if (mlModel == null)
            mlModel = FindObjectOfType<BarracudaMLModel>();

        if (mlModel == null)
        {
            Debug.LogWarning("⚠ BarracudaMLModel component not found");
            Debug.LogWarning("  Add it to test inference");
            return;
        }

        if (!mlModel.IsModelLoaded)
        {
            Debug.LogError("✗ Model is not loaded");
            return;
        }

        // Create a test texture
        Texture2D testTexture = new Texture2D(224, 224, TextureFormat.RGB24, false);
        
        // Fill with random colors (for testing)
        for (int y = 0; y < 224; y++)
        {
            for (int x = 0; x < 224; x++)
            {
                testTexture.SetPixel(x, y, new Color(
                    Random.value, 
                    Random.value, 
                    Random.value
                ));
            }
        }
        testTexture.Apply();

        // Run inference
        DetectionResponse response = mlModel.RunInference(testTexture);

        if (response.success)
        {
            Debug.Log($"✓ Inference successful!");
            Debug.Log($"  Detections: {response.detections.Count}");
            foreach (var detection in response.detections)
            {
                Debug.Log($"  - {detection.trash_type}: {detection.confidence:F2}");
            }
        }
        else
        {
            Debug.LogError($"✗ Inference failed: {response.error}");
        }

        Destroy(testTexture);
    }
}
