using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages ML-based trash detection and spawning
/// Integrates with AR camera feed and ML model
/// </summary>
public class MLTrashDetectionManager : MonoBehaviour
{
    public static MLTrashDetectionManager instance;

    [SerializeField] private ARCameraManager arCameraManager;
    [SerializeField] private float detectionInterval = 2f; // Detect every 2 seconds
    [SerializeField] private int maxTrashPerDetection = 3;
    
    private TrashDetectionAPI detectionAPI;
    private Coroutine detectionCoroutine;
    private bool isDetecting = false;

    [System.Serializable]
    public class TrashSpawnConfig
    {
        public string trashType;
        public GameObject prefab;
        public int spawnCount = 1;
    }
    
    public List<TrashSpawnConfig> trashConfigs = new List<TrashSpawnConfig>();

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        detectionAPI = TrashDetectionAPI.instance;
        if (detectionAPI == null)
        {
            detectionAPI = GetComponent<TrashDetectionAPI>();
            if (detectionAPI == null)
            {
                Debug.LogError("[MLTrashDetectionManager] TrashDetectionAPI not found!");
                return;
            }
        }

        // Get AR Camera Manager if not assigned
        if (arCameraManager == null)
        {
            arCameraManager = FindObjectOfType<ARCameraManager>();
        }

        // Start continuous trash detection
        StartTrashDetection();
    }

    public void StartTrashDetection()
    {
        if (detectionCoroutine != null)
            StopCoroutine(detectionCoroutine);
        
        detectionCoroutine = StartCoroutine(TrashDetectionLoop());
        Debug.Log("[MLTrashDetectionManager] Trash detection started");
    }

    public void StopTrashDetection()
    {
        if (detectionCoroutine != null)
        {
            StopCoroutine(detectionCoroutine);
            detectionCoroutine = null;
        }
        Debug.Log("[MLTrashDetectionManager] Trash detection stopped");
    }

    IEnumerator TrashDetectionLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(detectionInterval);

            if (!isDetecting)
            {
                yield return StartCoroutine(DetectAndSpawnTrash());
            }
        }
    }

    IEnumerator DetectAndSpawnTrash()
    {
        isDetecting = true;

        // Capture current camera frame
        Texture2D cameraFrame = CaptureARCameraFrame();
        
        if (cameraFrame == null)
        {
            isDetecting = false;
            yield break;
        }

        // Run ML detection
        bool detectionComplete = false;
        DetectionResponse detectionResult = null;

        yield return StartCoroutine(detectionAPI.DetectTrash(cameraFrame, (response) =>
        {
            detectionResult = response;
            detectionComplete = true;
        }));

        // Wait for detection to complete
        float timeout = 5f;
        float elapsed = 0f;
        while (!detectionComplete && elapsed < timeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Process detection results
        if (detectionResult != null && detectionResult.success)
        {
            int spawnedCount = 0;
            foreach (var detection in detectionResult.detections)
            {
                if (spawnedCount >= maxTrashPerDetection) break;
                
                // Only spawn if confidence is above threshold
                if (detection.confidence >= detectionAPI.confidenceThreshold)
                {
                    SpawnTrashFromDetection(detection);
                    spawnedCount++;
                }
            }

            Debug.Log($"[MLTrashDetectionManager] Spawned {spawnedCount} trash items from detection");
        }
        else
        {
            Debug.LogWarning("[MLTrashDetectionManager] Detection failed or returned no results");
        }

        Destroy(cameraFrame);
        isDetecting = false;
    }

    void SpawnTrashFromDetection(DetectionResult detection)
    {
        // Map detection type to trash type
        string dragonType = detectionAPI.MapDetectionToTrashType(detection.trash_type);
        
        // Find corresponding prefab
        GameObject trashPrefab = GetTrashPrefab(dragonType);
        if (trashPrefab == null)
        {
            Debug.LogWarning($"[MLTrashDetectionManager] No prefab found for trash type: {dragonType}");
            return;
        }

        // Calculate spawn position from bounding box
        // Use detection bbox to position trash in the world
        Vector3 spawnPosition = CalculateSpawnPositionFromBBox(detection.bbox);

        // Instantiate trash
        GameObject trashInstance = Instantiate(trashPrefab, spawnPosition, Quaternion.identity);
        
        // Set the monster name
        Collectible collectible = trashInstance.GetComponent<Collectible>();
        if (collectible != null)
        {
            collectible.monsterName = dragonType;
        }

        Debug.Log($"[MLTrashDetectionManager] Spawned {dragonType} at {spawnPosition} (Confidence: {detection.confidence:F2})");
    }

    Vector3 CalculateSpawnPositionFromBBox(float[] bbox)
    {
        // bbox = [x, y, width, height] in normalized coordinates
        // Convert to world position relative to camera
        
        if (bbox == null || bbox.Length < 4)
            return Vector3.zero;

        // Get camera position
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
            return Vector3.zero;

        // Map bbox to screen coordinates
        float screenX = bbox[0] * Screen.width;
        float screenY = bbox[1] * Screen.height;

        // Convert to world space
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(
            new Vector3(screenX, screenY, 5f) // 5 units in front of camera
        );

        return worldPos;
    }

    Texture2D CaptureARCameraFrame()
    {
        if (arCameraManager == null)
        {
            Debug.LogWarning("[MLTrashDetectionManager] AR Camera Manager not available");
            return null;
        }

        // Capture current frame from AR camera
        // This is a simplified version - actual implementation depends on your AR setup
        
        RenderTexture rt = new RenderTexture(640, 480, 24);
        Graphics.Blit(Texture2D.whiteTexture, rt);

        RenderTexture.active = rt;
        Texture2D screenshot = new Texture2D(640, 480, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, 640, 480), 0, 0);
        screenshot.Apply();
        RenderTexture.active = null;
        Destroy(rt);

        return screenshot;
    }

    GameObject GetTrashPrefab(string trashType)
    {
        foreach (var config in trashConfigs)
        {
            if (config.trashType == trashType && config.prefab != null)
                return config.prefab;
        }
        return null;
    }

    public void OnDestroy()
    {
        StopTrashDetection();
    }
}
