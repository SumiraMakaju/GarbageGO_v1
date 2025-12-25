using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System.Collections;
public class Collectible : MonoBehaviour
{
    public string monsterName = "Garbage Monster";
    private Collider objectCollider;
    private bool isDestroyed = false;
    private Animator animator;
    private Vector3 spawnPosition;
    private Rigidbody rb;

    void Start()
    {
        // Store spawn position to maintain it
        spawnPosition = transform.position;
        
        // Get or create collider
        objectCollider = GetComponent<Collider>();
        if (objectCollider == null)
        {
            SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
            sphereCollider.radius = 5f;
            sphereCollider.center = Vector3.zero;
            objectCollider = sphereCollider;
        }
        
        objectCollider.isTrigger = false;
        
        // Lock Rigidbody in place
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints.FreezeAll;
            rb.useGravity = false;
        }
        
        // Get animator if it exists
        animator = GetComponent<Animator>();
        
        Debug.Log($"[Collectible] Initialized: {monsterName} on {gameObject.name} at position {spawnPosition}");
    }
    
    void LateUpdate()
    {
        // Ensure dragon stays at spawn position despite any physics or animation
        if (!isDestroyed && transform.position != spawnPosition)
        {
            transform.position = spawnPosition;
        }
    }

    void Update()
    {
        if (isDestroyed) return;

        // Handle touch input for AR (mobile) - New Input System
        #if UNITY_ANDROID || UNITY_IOS
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            if (Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            {
                Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
                HandleInput(touchPosition);
            }
        }
        #endif
        
        // Handle mouse click for editor/desktop testing - New Input System
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandleInput(Mouse.current.position.ReadValue());
        }
    }

    void HandleInput(Vector2 screenPosition)
    {
        if (Camera.main == null)
        {
            Debug.LogError("[Collectible] No main camera found!");
            return;
        }
        
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, 1000f);

        Debug.Log($"[Collectible] Raycast at {screenPosition}: {hits.Length} hits");
        
        foreach (RaycastHit hit in hits)
        {
            Debug.Log($"[Collectible] Hit: {hit.collider.gameObject.name}");
            
            if (hit.collider.gameObject == gameObject)
            {
                Debug.Log($"[Collectible] ✓ HIT {gameObject.name} ({monsterName})");
                CollectMonster();
                return;
            }
        }
    }

    void CollectMonster()
    {
        if (isDestroyed)
        {
            Debug.LogWarning($"[Collectible] {gameObject.name} already destroyed!");
            return;
        }
        
        isDestroyed = true;

        // Disable collider immediately to prevent double-tap
        objectCollider.enabled = false;
        
        // Disable Rigidbody to prevent any physics interaction
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Update score
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.AddMonster(monsterName);
            Debug.Log($"[Collectible] 🎉 COLLECTED: {monsterName}! Score: {ScoreManager.instance.score}");
        }
        else
        {
            Debug.LogError("[Collectible] ScoreManager instance not found!");
        }

        // Start capture animation (Pokemon Go style)
        StartCoroutine(CaptureAnimation());
    }

    IEnumerator CaptureAnimation()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 originalPosition = transform.position;
        Vector3 peakPosition = originalPosition + Vector3.up * 0.8f; // Rise up first
        Vector3 portalCenter = originalPosition + Vector3.up * 0.5f; // Portal center point
        
        float totalDuration = 1.8f;
        float elapsed = 0f;
        
        while (elapsed < totalDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / totalDuration;
            
            // Use cubic ease-in for smooth acceleration
            float easeProgress = progress * progress * progress;
            
            // Phase 1 (0-0.3): Rise up
            Vector3 targetPosition;
            if (progress < 0.3f)
            {
                float phaseProgress = progress / 0.3f;
                float phaseEase = phaseProgress * phaseProgress;
                targetPosition = Vector3.Lerp(originalPosition, peakPosition, phaseEase);
            }
            // Phase 2 (0.3-1): Fall and shrink into portal
            else
            {
                float phaseProgress = (progress - 0.3f) / 0.7f;
                float phaseEase = phaseProgress * phaseProgress * phaseProgress;
                targetPosition = Vector3.Lerp(peakPosition, portalCenter, phaseEase);
            }
            
            transform.position = targetPosition;
            
            // Scale down smoothly with easing
            float scaleProgress = Mathf.Lerp(1f, 0f, easeProgress);
            transform.localScale = originalScale * scaleProgress;
            
            // Rotate with acceleration (spiral effect)
            float rotation = progress * 360f * 2.5f; // 2.5 full rotations
            transform.Rotate(Vector3.forward, rotation, Space.Self);
            
            // Fade out smoothly
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                Color color = renderer.material.color;
                color.a = Mathf.Lerp(1f, 0f, easeProgress);
                renderer.material.color = color;
            }
            
            yield return null;
        }
        
        // Ensure invisible
        Renderer[] finalRenderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in finalRenderers)
        {
            renderer.enabled = false;
        }
        
        // Show +1 floating text at original position before destroying
        ShowFloatingText(originalPosition);
        
        yield return new WaitForSeconds(0.15f);
        
        // Destroy the game object
        Destroy(gameObject);
    }

    void ShowFloatingText(Vector3 position)
    {
        // Create a canvas for the floating text
        GameObject canvasObj = new GameObject("FloatingTextCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvasObj.AddComponent<GraphicRaycaster>();
        
        RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(100, 100);
        canvasRect.position = position + Vector3.up * 0.3f;
        canvasRect.localScale = new Vector3(0.005f, 0.005f, 0.005f);

        // Create the text object
        GameObject textObj = new GameObject("FloatingText");
        textObj.transform.SetParent(canvasObj.transform, false);
        
        TextMeshProUGUI textMesh = textObj.AddComponent<TextMeshProUGUI>();
        textMesh.text = "+1";
        textMesh.fontSize = 40;
        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.color = new Color(1f, 0.84f, 0f, 1f); // Gold color
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(100, 100);
        textRect.anchoredPosition = Vector2.zero;

        Debug.Log($"[Collectible] Showing +1 floating text at position {position}");
        
        // Start the floating animation
        StartCoroutine(FloatingTextAnimation(canvasObj, position, 1.8f));
    }

    IEnumerator FloatingTextAnimation(GameObject canvasObj, Vector3 startPos, float duration)
    {
        float elapsedTime = 0f;
        Vector3 endPosition = startPos + Vector3.up * 2.5f; // Float up 2.5 units
        Vector3 startScale = canvasObj.transform.localScale;

        TextMeshProUGUI textMesh = canvasObj.GetComponentInChildren<TextMeshProUGUI>();

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;

            // Smooth ease-out curve (quintic)
            float easeProgress = 1f - Mathf.Pow(1f - progress, 3f);

            // Move upward
            canvasObj.transform.position = Vector3.Lerp(startPos, endPosition, easeProgress);

            // Scale animation: grow slightly then shrink
            float scaleProgress;
            if (progress < 0.2f)
            {
                // Grow for first 20%
                scaleProgress = Mathf.Lerp(1f, 1.3f, progress / 0.2f);
            }
            else
            {
                // Shrink for remaining 80%
                scaleProgress = Mathf.Lerp(1.3f, 0.5f, (progress - 0.2f) / 0.8f);
            }
            canvasObj.transform.localScale = startScale * scaleProgress;

            // Fade out
            Color color = textMesh.color;
            color.a = Mathf.Lerp(1f, 0f, progress);
            textMesh.color = color;

            yield return null;
        }

        // Clean up
        Destroy(canvasObj);
    }
}

