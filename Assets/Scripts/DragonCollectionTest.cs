using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Test script to verify dragon collection system is working correctly.
/// Attach this to any GameObject in the scene to see debug output.
/// </summary>
public class DragonCollectionTest : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== DRAGON COLLECTION SYSTEM TEST ===");
        
        // Test 1: Verify ScoreManager exists
        if (ScoreManager.instance != null)
        {
            Debug.Log("✓ ScoreManager initialized");
            Debug.Log($"  Current score: {ScoreManager.instance.score}");
        }
        else
        {
            Debug.LogError("✗ ScoreManager NOT found - Dragons won't be collected!");
        }
        
        // Test 2: Find all Collectible components
        Collectible[] collectibles = FindObjectsOfType<Collectible>();
        Debug.Log($"✓ Found {collectibles.Length} Collectible components in scene");
        
        // Test 3: Find all dragons and check their setup
        foreach (Collectible collectible in collectibles)
        {
            Collider collider = collectible.GetComponent<Collider>();
            if (collider == null)
            {
                Debug.LogWarning($"  ⚠ {collectible.gameObject.name} missing collider!");
            }
            else if (!collider.enabled)
            {
                Debug.LogWarning($"  ⚠ {collectible.gameObject.name} collider disabled!");
            }
            else
            {
                Debug.Log($"  ✓ {collectible.gameObject.name} ({collectible.monsterName}) - Ready!");
            }
        }
        
        // Test 4: Check for main camera
        if (Camera.main != null)
        {
            Debug.Log("✓ Main camera found");
        }
        else
        {
            Debug.LogError("✗ Main camera NOT found - Raycasting will fail!");
        }
        
        // Test 5: Input system check
        #if UNITY_ANDROID || UNITY_IOS
        Debug.Log("✓ Platform: Android/iOS - Using touch input");
        #else
        Debug.Log("✓ Platform: Editor/Desktop - Using mouse input");
        #endif
        
        Debug.Log("=== TEST COMPLETE ===");
    }
    
    void Update()
    {
        // Show input debug info - New Input System
        #if UNITY_ANDROID || UNITY_IOS
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            Debug.Log($"[Input] Touch detected at: {touchPosition}");
        }
        #endif
        
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Debug.Log($"[Input] Mouse click at: {Mouse.current.position.ReadValue()}");
        }
    }
}
