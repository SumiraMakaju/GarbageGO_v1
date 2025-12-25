using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Text;

[System.Serializable]
public class DetectionResult
{
    public string trash_type;      // Type of trash detected
    public float confidence;       // Confidence score (0-1)
    public float[] bbox;           // Bounding box [x, y, width, height]
}

[System.Serializable]
public class DetectionResponse
{
    public List<DetectionResult> detections;
    public bool success;
    public string error;
}

public class TrashDetectionAPI : MonoBehaviour
{
    public static TrashDetectionAPI instance;
    
    public string apiUrl = "http://YOUR_SERVER/detect";
    public bool useLocalModel = true; // Toggle between local ML and API
    public float confidenceThreshold = 0.6f;
    
    // Local ML Model (for offline detection)
    private object mlModel; // Barracuda model would go here
    
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Send camera frame to ML model for trash detection
    /// </summary>
    public IEnumerator DetectTrash(Texture2D cameraFrame, System.Action<DetectionResponse> onComplete)
    {
        if (useLocalModel)
        {
            yield return StartCoroutine(DetectWithLocalModel(cameraFrame, onComplete));
        }
        else
        {
            yield return StartCoroutine(DetectWithAPI(cameraFrame, onComplete));
        }
    }

    /// <summary>
    /// Detect trash using local ML model (offline, faster)
    /// </summary>
    private IEnumerator DetectWithLocalModel(Texture2D image, System.Action<DetectionResponse> onComplete)
    {
        yield return new WaitForEndOfFrame();
        
        DetectionResponse response = new DetectionResponse
        {
            success = false,
            detections = new List<DetectionResult>()
        };
        
        // Use Barracuda ML model if available
        if (BarracudaMLModel.instance != null)
        {
            response = BarracudaMLModel.instance.RunInference(image);
            Debug.Log($"[TrashDetectionAPI] Barracuda inference: {response.detections.Count} detections found");
        }
        else
        {
            // Fallback: Mock detection for testing
            response.success = true;
            response.detections = new List<DetectionResult>
            {
                new DetectionResult 
                { 
                    trash_type = "plastic_bottle",
                    confidence = 0.95f,
                    bbox = new float[] { 0.2f, 0.2f, 0.15f, 0.2f }
                }
            };
            Debug.LogWarning("[TrashDetectionAPI] BarracudaMLModel not found, using mock detection");
        }
        
        onComplete?.Invoke(response);
    }

    /// <summary>
    /// Detect trash using remote API (requires internet)
    /// </summary>
    private IEnumerator DetectWithAPI(Texture2D image, System.Action<DetectionResponse> onComplete)
    {
        byte[] jpg = image.EncodeToJPG();
        WWWForm form = new WWWForm();
        form.AddBinaryData("image", jpg, "frame.jpg", "image/jpeg");

        UnityWebRequest req = UnityWebRequest.Post(apiUrl, form);
        yield return req.SendWebRequest();

        DetectionResponse response = new DetectionResponse();

        if (req.result == UnityWebRequest.Result.Success)
        {
            try
            {
                response = JsonUtility.FromJson<DetectionResponse>(req.downloadHandler.text);
                response.success = true;
                Debug.Log("[TrashDetectionAPI] API detection successful: " + response.detections.Count + " objects");
            }
            catch (System.Exception e)
            {
                response.success = false;
                response.error = "JSON parse error: " + e.Message;
                Debug.LogError("[TrashDetectionAPI] Failed to parse API response: " + e.Message);
            }
        }
        else
        {
            response.success = false;
            response.error = req.error;
            Debug.LogError("[TrashDetectionAPI] API error: " + req.error);
        }

        onComplete?.Invoke(response);
    }

    /// <summary>
    /// Map detection result to trash type for spawning
    /// </summary>
    public string MapDetectionToTrashType(string detectionType)
    {
        // Map ML model trash types to game trash types
        var typeMap = new Dictionary<string, string>
        {
            { "plastic_bottle", "DragonNightmare_Blue" },
            { "plastic_bag", "DragonNightmare_Green" },
            { "can", "DragonSoulEater_Blue" },
            { "metal_waste", "DragonSoulEater_Red" },
            { "paper", "DragonTerrorBringer_Purple" },
            { "cardboard", "DragonTerrorBringer_Blue" },
            { "glass", "DragonUsurper_Green" },
            { "organic", "DragonUsurper_Purple" },
        };

        if (typeMap.ContainsKey(detectionType))
            return typeMap[detectionType];
        
        return "DragonNightmare_Blue"; // Default fallback
    }
}
