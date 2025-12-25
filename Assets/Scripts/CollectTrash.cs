using UnityEngine;
using UnityEngine.InputSystem;

public class CollectTrash : MonoBehaviour
{
    public string monsterName = "Trash";
    private Collider objectCollider;
    private bool isDestroyed = false;

    void Start()
    {
        // Ensure the object has a collider for raycasting
        objectCollider = GetComponent<Collider>();
        if (objectCollider == null)
        {
            SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
            sphereCollider.radius = 5f;
            sphereCollider.center = Vector3.zero;
            objectCollider = sphereCollider;
        }
        
        objectCollider.isTrigger = false;
        Debug.Log($"[CollectTrash] Initialized: {monsterName} on {gameObject.name}");
    }

    void Update()
    {
        if (isDestroyed) return;

        // Handle touch input for AR (mobile) - New Input System
        #if UNITY_ANDROID || UNITY_IOS
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            HandleInput(touchPosition);
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
            Debug.LogError("[CollectTrash] No main camera found!");
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, 1000f);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject == gameObject)
            {
                Debug.Log($"[CollectTrash] ✓ HIT {gameObject.name} ({monsterName})");
                CollectMonster();
                return;
            }
        }
    }

    void CollectMonster()
    {
        if (isDestroyed)
        {
            Debug.LogWarning($"[CollectTrash] {gameObject.name} already destroyed!");
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
            Debug.Log($"[CollectTrash] 🎉 COLLECTED: {monsterName}! Score: {ScoreManager.instance.score}");
        }
        else
        {
            Debug.LogError("[CollectTrash] ScoreManager instance not found!");
        }

        // Hide immediately by disabling renderer
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = false;
        }

        // Destroy after a short delay
        Destroy(gameObject, 0.2f);
    }
}