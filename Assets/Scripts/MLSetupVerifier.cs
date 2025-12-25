using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Verifies ML model setup and configuration
/// Helps debug any loading issues
/// </summary>
public class MLSetupVerifier : MonoBehaviour
{
    void Start()
    {
        VerifyMLSetup();
    }

    void VerifyMLSetup()
    {
        Debug.Log("\n" + new string('=', 60));
        Debug.Log("🔍 ML MODEL SETUP VERIFICATION");
        Debug.Log(new string('=', 60));

        // Check 1: Model file exists
        VerifyModelFile();

        // Check 2: Barracuda package
        VerifyBarracudaPackage();

        // Check 3: BarracudaMLModel component
        VerifyBarracudaMLModelComponent();

        // Check 4: TrashDetectionAPI component
        VerifyTrashDetectionAPIComponent();

        // Check 5: MLTrashDetectionManager component
        VerifyMLTrashDetectionManagerComponent();

        Debug.Log(new string('=', 60) + "\n");
    }

    void VerifyModelFile()
    {
        Debug.Log("\n[1/5] Checking model file...");
        
        var model = Resources.Load("Models/waste_classifier");
        if (model != null)
        {
            Debug.Log("✓ Model file found: waste_classifier.onnx");
            Debug.Log("  Location: Assets/Resources/Models/waste_classifier.onnx");
        }
        else
        {
            Debug.LogError("✗ Model file NOT found!");
            Debug.LogError("  Expected: Assets/Models/waste_classifier.onnx");
            Debug.LogError("  To fix: Move waste_classifier.onnx to Assets/Models/ folder");
            Debug.LogError("  Note: The file must be in Assets/Models/ (not Assets/Resources/Models/)");
        }
    }

    void VerifyBarracudaPackage()
    {
        Debug.Log("\n[2/5] Checking Barracuda package...");
        
#if UNITY_BARRACUDA
        Debug.Log("✓ Barracuda package is installed");
        Debug.Log("  You can use: com.unity.barracuda");
#else
        Debug.LogError("✗ Barracuda package NOT installed!");
        Debug.LogError("  To install:");
        Debug.LogError("  1. Window → Package Manager");
        Debug.LogError("  2. Click '+' button → Add package by name");
        Debug.LogError("  3. Enter: com.unity.barracuda");
        Debug.LogError("  4. Click 'Add'");
#endif
    }

    void VerifyBarracudaMLModelComponent()
    {
        Debug.Log("\n[3/5] Checking BarracudaMLModel component...");
        
        BarracudaMLModel mlModel = FindObjectOfType<BarracudaMLModel>();
        if (mlModel != null)
        {
            Debug.Log("✓ BarracudaMLModel found in scene");
            if (mlModel.IsModelLoaded)
            {
                Debug.Log("✓ Model is loaded and ready");
            }
            else
            {
                Debug.LogWarning("⚠ Model is not loaded yet");
                Debug.LogWarning("  Check console for load errors");
            }
        }
        else
        {
            Debug.LogError("✗ BarracudaMLModel NOT found in scene!");
            Debug.LogError("  To fix:");
            Debug.LogError("  1. Create empty GameObject: 'MLSystem'");
            Debug.LogError("  2. Add BarracudaMLModel.cs component");
            Debug.LogError("  3. In Inspector, set Model Path to: Models/waste_classifier");
        }
    }

    void VerifyTrashDetectionAPIComponent()
    {
        Debug.Log("\n[4/5] Checking TrashDetectionAPI component...");
        
        TrashDetectionAPI detectionAPI = FindObjectOfType<TrashDetectionAPI>();
        if (detectionAPI != null)
        {
            Debug.Log("✓ TrashDetectionAPI found in scene");
        }
        else
        {
            Debug.LogWarning("⚠ TrashDetectionAPI NOT found in scene");
            Debug.LogWarning("  Optional: Add TrashDetectionAPI.cs to the MLSystem GameObject");
        }
    }

    void VerifyMLTrashDetectionManagerComponent()
    {
        Debug.Log("\n[5/5] Checking MLTrashDetectionManager component...");
        
        MLTrashDetectionManager detectionManager = FindObjectOfType<MLTrashDetectionManager>();
        if (detectionManager != null)
        {
            Debug.Log("✓ MLTrashDetectionManager found in scene");
        }
        else
        {
            Debug.LogWarning("⚠ MLTrashDetectionManager NOT found in scene");
            Debug.LogWarning("  Optional: Add MLTrashDetectionManager.cs for auto-detection");
        }
    }
}
