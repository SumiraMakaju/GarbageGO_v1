using UnityEngine;
using System.Collections.Generic;
using System.Linq;

#if UNITY_BARRACUDA
using Unity.Barracuda;
#endif

/// <summary>
/// Handles Barracuda ML model loading and inference for trash detection
/// </summary>
public class BarracudaMLModel : MonoBehaviour
{
    public static BarracudaMLModel instance;

    [SerializeField] private string modelPath = "Models/waste_classifier"; // Your actual model name
    [SerializeField] private int inputWidth = 224;
    [SerializeField] private int inputHeight = 224;
    [SerializeField] private float confidenceThreshold = 0.6f;
    [SerializeField] private bool useGPU = true;

#if UNITY_BARRACUDA
    private Model model;
    private IWorker worker;
    private bool isModelLoaded = false;
#endif

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        LoadModel();
    }

    /// <summary>
    /// Load Barracuda model from Resources
    /// </summary>
    void LoadModel()
    {
#if UNITY_BARRACUDA
        try
        {
            // Load model from Assets/Resources/Models/
            model = Resources.Load<Model>(modelPath);
            
            if (model == null)
            {
                Debug.LogError($"[BarracudaMLModel] ❌ Failed to load model from path: {modelPath}");
                Debug.LogError($"[BarracudaMLModel] Expected location: Assets/Resources/{modelPath}.onnx");
                Debug.LogError($"[BarracudaMLModel] Actual file: Assets/Models/waste_classifier.onnx");
                return;
            }

            // Print model details
            Debug.Log($"[BarracudaMLModel] ✓ Model loaded: {model.name}");
            Debug.Log($"[BarracudaMLModel] Input shape: {inputWidth}x{inputHeight}");
            
            // Show all inputs and outputs
            Debug.Log($"[BarracudaMLModel] Model inputs: {string.Join(", ", model.inputs.ToArray())}");
            Debug.Log($"[BarracudaMLModel] Model outputs: {string.Join(", ", model.outputs.ToArray())}");

            // Create worker for GPU/CPU inference
            Backend backend = useGPU ? Backend.GPUCompute : Backend.CPU;
            worker = ModelBuilder.CreateWorker(model, backend);
            isModelLoaded = true;

            Debug.Log($"[BarracudaMLModel] ✓ Worker created successfully (Backend: {backend})");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[BarracudaMLModel] ❌ Failed to load model: {ex.Message}\n{ex.StackTrace}");
        }
#else
        Debug.LogError("[BarracudaMLModel] ❌ Barracuda package not installed!");
        Debug.LogError("[BarracudaMLModel] Install via Package Manager:");
        Debug.LogError("[BarracudaMLModel] Window → Package Manager → Add package by name → com.unity.barracuda");
#endif
    }

    /// <summary>
    /// Run inference on camera frame
    /// </summary>
    public DetectionResponse RunInference(Texture2D input)
    {
        DetectionResponse response = new DetectionResponse
        {
            detections = new List<DetectionResult>(),
            success = false
        };

#if UNITY_BARRACUDA
        if (!isModelLoaded || worker == null)
        {
            response.error = "Model not loaded";
            Debug.LogWarning("[BarracudaMLModel] Model is not loaded");
            return response;
        }

        if (input == null)
        {
            response.error = "Input texture is null";
            Debug.LogWarning("[BarracudaMLModel] Input texture is null");
            return response;
        }

        try
        {
            // Prepare input tensor
            Tensor inputTensor = TransformInput(input);
            
            if (inputTensor == null)
            {
                response.error = "Failed to create input tensor";
                return response;
            }

            // Execute model
            worker.Execute(inputTensor);

            // Get output and parse detections
            response.detections = ParseOutput(worker);
            response.success = response.detections.Count > 0;

            inputTensor.Dispose();

            Debug.Log($"[BarracudaMLModel] ✓ Inference complete: {response.detections.Count} detections");
        }
        catch (System.Exception ex)
        {
            response.error = ex.Message;
            Debug.LogError($"[BarracudaMLModel] ❌ Inference error: {ex.Message}");
        }
#else
        response.error = "Barracuda not available";
#endif

        return response;
    }

#if UNITY_BARRACUDA
    /// <summary>
    /// Transform input image to tensor
    /// </summary>
    Tensor TransformInput(Texture2D input)
    {
        try
        {
            // Resize to model input size
            Texture2D resized = ResizeTexture(input, inputWidth, inputHeight);
            
            if (resized == null)
            {
                Debug.LogError("[BarracudaMLModel] Failed to resize texture");
                return null;
            }

            // Convert to float array and normalize
            float[] data = new float[inputWidth * inputHeight * 3];
            Color[] pixels = resized.GetPixels();
            
            for (int i = 0; i < pixels.Length; i++)
            {
                // Normalize RGB values to 0-1 range
                data[i * 3] = pixels[i].r;      // R channel
                data[i * 3 + 1] = pixels[i].g;  // G channel
                data[i * 3 + 2] = pixels[i].b;  // B channel
            }

            Tensor tensor = new Tensor(1, inputHeight, inputWidth, 3, data);
            Destroy(resized);
            
            return tensor;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[BarracudaMLModel] Error transforming input: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Resize texture to target dimensions
    /// </summary>
    Texture2D ResizeTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        RenderTexture rt = new RenderTexture(targetWidth, targetHeight, 24);
        Graphics.Blit(source, rt);
        
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = rt;
        
        Texture2D result = new Texture2D(targetWidth, targetHeight, TextureFormat.RGB24, false);
        result.ReadPixels(new Rect(0, 0, targetWidth, targetHeight), 0, 0);
        result.Apply();
        
        RenderTexture.active = previous;
        Destroy(rt);
        
        return result;
    }

    /// <summary>
    /// Parse model output to detection results
    /// </summary>
    List<DetectionResult> ParseOutput(IWorker worker)
    {
        List<DetectionResult> detections = new List<DetectionResult>();

        try
        {
            // Get the first output tensor
            var outputNames = model.outputs;
            if (outputNames.Count == 0)
            {
                Debug.LogWarning("[BarracudaMLModel] No output tensors found in model");
                return detections;
            }

            string outputName = outputNames[0];
            Tensor outputTensor = worker.PeekOutput(outputName);
            
            if (outputTensor == null)
            {
                Debug.LogWarning($"[BarracudaMLModel] Output tensor '{outputName}' is null");
                return detections;
            }

            // Read output data
            float[] output = outputTensor.data.Download(outputTensor.shape);
            
            Debug.Log($"[BarracudaMLModel] Output shape: {outputTensor.shape}, Data length: {output.Length}");

            // Parse based on output shape
            // For classification model: [batch, num_classes]
            // For detection model: [batch, boxes, features]
            
            if (output.Length >= 8) // Typical classification has multiple classes
            {
                // Assuming output is [batch, class_scores...]
                int numClasses = output.Length;
                for (int i = 0; i < numClasses; i++)
                {
                    if (output[i] > confidenceThreshold)
                    {
                        DetectionResult detection = new DetectionResult
                        {
                            trash_type = GetTrashTypeFromClass(i),
                            confidence = output[i],
                            bbox = new float[] { 0.25f, 0.25f, 0.5f, 0.5f } // Default center box
                        };
                        detections.Add(detection);
                    }
                }
            }

            if (detections.Count == 0)
            {
                Debug.LogWarning($"[BarracudaMLModel] No detections above threshold ({confidenceThreshold})");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[BarracudaMLModel] Error parsing output: {ex.Message}");
        }

        return detections;
    }

    /// <summary>
    /// Map class ID to trash type name
    /// </summary>
    string GetTrashTypeFromClass(int classId)
    {
        // Adjust based on your model's class mapping
        Dictionary<int, string> classMap = new Dictionary<int, string>
        {
            { 0, "plastic_bottle" },
            { 1, "plastic_bag" },
            { 2, "can" },
            { 3, "metal_waste" },
            { 4, "paper" },
            { 5, "cardboard" },
            { 6, "glass" },
            { 7, "organic" }
        };

        if (classMap.ContainsKey(classId))
            return classMap[classId];
        
        return "unknown_trash";
    }
#endif

    public bool IsModelLoaded
    {
        get
        {
#if UNITY_BARRACUDA
            return isModelLoaded;
#else
            return false;
#endif
        }
    }

    void OnDestroy()
    {
#if UNITY_BARRACUDA
        worker?.Dispose();
        Debug.Log("[BarracudaMLModel] Worker disposed");
#endif
    }
}

