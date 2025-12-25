using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Debug script to verify PrefabCreator setup and manually trigger spawning
/// </summary>
public class PrefabCreatorDebug : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== PREFAB CREATOR DEBUG ===");
        
        PrefabCreator creator = FindObjectOfType<PrefabCreator>();
        if (creator == null)
        {
            Debug.LogError("✗ PrefabCreator not found in scene!");
            return;
        }
        
        Debug.Log("✓ PrefabCreator found");
        
        // Check characterVariants
        if (creator.characterVariants == null)
        {
            Debug.LogError("✗ characterVariants is NULL!");
            return;
        }
        
        if (creator.characterVariants.Length == 0)
        {
            Debug.LogError("✗ characterVariants array is EMPTY!");
            Debug.Log("  → Open PrefabCreator in inspector and add character variants");
            return;
        }
        
        Debug.Log($"✓ Found {creator.characterVariants.Length} character variants:");
        
        int validCount = 0;
        for (int i = 0; i < creator.characterVariants.Length; i++)
        {
            var variant = creator.characterVariants[i];
            if (variant == null)
            {
                Debug.LogWarning($"  [{i}] NULL variant!");
                continue;
            }
            
            if (variant.prefab == null)
            {
                Debug.LogWarning($"  [{i}] {variant.characterName} - NO PREFAB ASSIGNED!");
                continue;
            }
            
            Debug.Log($"  [{i}] {variant.characterName} ✓");
            validCount++;
        }
        
        if (validCount == 0)
        {
            Debug.LogError("✗ NO valid prefabs assigned!");
            Debug.Log("  → Drag prefabs from Assets/FourEvilDragonsHP/Prefab/ into the array");
            return;
        }
        
        Debug.Log($"\n✓ {validCount} valid variants ready");
        Debug.Log($"✓ Monster count: {creator.monsterCount}");
        Debug.Log($"✓ Spawn radius: {creator.spawnRadius}");
        Debug.Log($"✓ Manual spawn mode: {creator.manualSpawnMode}");
        
        if (creator.manualSpawnMode)
        {
            Debug.Log("\n→ Press SPACE to spawn monsters (Manual Mode)");
        }
        else
        {
            Debug.Log("\n→ Waiting for AR image detection...");
        }
    }
    
    void Update()
    {
        // Alt + S for forced debug spawn - New Input System
        if (Keyboard.current != null && 
            Keyboard.current.sKey.wasPressedThisFrame && 
            Keyboard.current.leftAltKey.isPressed)
        {
            PrefabCreator creator = FindObjectOfType<PrefabCreator>();
            if (creator != null)
            {
                Debug.Log("Forced spawn triggered!");
                // Call private method via reflection for testing
                var method = creator.GetType().GetMethod("SpawnMonsters", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (method != null)
                {
                    method.Invoke(creator, new object[] { creator.transform });
                }
            }
        }
    }
}
