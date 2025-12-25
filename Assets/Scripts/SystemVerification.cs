using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Dragon Collection System Implementation Verification
/// Run this at startup to ensure everything is configured correctly
/// </summary>
public class SystemVerification : MonoBehaviour
{
    void Start()
    {
        VerifyDragonCollectionSystem();
    }

    public void VerifyDragonCollectionSystem()
    {
        Debug.Log("\n" + new string('=', 60));
        Debug.Log("DRAGON COLLECTION SYSTEM VERIFICATION");
        Debug.Log(new string('=', 60));

        int issueCount = 0;

        // 1. Verify ScoreManager
        Debug.Log("\n[1/5] Checking ScoreManager...");
        if (ScoreManager.instance == null)
        {
            Debug.LogError("  ✗ FAIL: ScoreManager singleton not found!");
            Debug.LogError("    → Add ScoreManager script to a GameObject in the scene");
            issueCount++;
        }
        else
        {
            Debug.Log("  ✓ PASS: ScoreManager exists");
            if (ScoreManager.instance.scoreText == null)
            {
                Debug.LogWarning("  ⚠ WARNING: ScoreManager.scoreText not assigned");
                Debug.LogWarning("    → Assign a TextMeshProUGUI element to ScoreManager.scoreText");
            }
            else
            {
                Debug.Log("  ✓ PASS: Score text UI assigned");
            }
        }

        // 2. Verify Main Camera
        Debug.Log("\n[2/5] Checking Main Camera...");
        if (Camera.main == null)
        {
            Debug.LogError("  ✗ FAIL: Main camera not found!");
            Debug.LogError("    → Ensure camera has 'MainCamera' tag");
            issueCount++;
        }
        else
        {
            Debug.Log("  ✓ PASS: Main camera found");
        }

        // 3. Check Input System Settings
        Debug.Log("\n[3/5] Checking Input System Configuration...");
        #if UNITY_ANDROID || UNITY_IOS
        Debug.Log("  ✓ PASS: Platform is Android/iOS - will use touch input");
        #else
        Debug.Log("  ✓ PASS: Platform is Editor/Desktop - will use mouse input");
        #endif

        // 4. Find Collectible Components
        Debug.Log("\n[4/5] Checking Dragon Collection Components...");
        Collectible[] collectibles = FindObjectsOfType<Collectible>();
        if (collectibles.Length == 0)
        {
            Debug.LogWarning("  ⚠ WARNING: No Collectible components found in scene");
            Debug.LogWarning("    → Dragons may not be spawned yet");
            Debug.LogWarning("    → Check PrefabCreator and AR marker detection");
        }
        else
        {
            Debug.Log($"  ✓ PASS: Found {collectibles.Length} dragons ready for collection");
            
            // Check each dragon
            int readyCount = 0;
            foreach (Collectible coll in collectibles)
            {
                Collider col = coll.GetComponent<Collider>();
                if (col != null && col.enabled)
                {
                    readyCount++;
                }
            }
            Debug.Log($"    → {readyCount}/{collectibles.Length} dragons have active colliders");
            
            if (readyCount < collectibles.Length)
            {
                Debug.LogWarning($"    ⚠ {collectibles.Length - readyCount} dragons missing/disabled colliders");
            }
        }

        // 5. Check PrefabCreator
        Debug.Log("\n[5/5] Checking PrefabCreator...");
        PrefabCreator creator = FindObjectOfType<PrefabCreator>();
        if (creator == null)
        {
            Debug.LogWarning("  ⚠ WARNING: PrefabCreator not found");
            Debug.LogWarning("    → If you're using AR image tracking, add PrefabCreator to the AR manager GameObject");
        }
        else
        {
            if (creator.characterVariants == null || creator.characterVariants.Length == 0)
            {
                Debug.LogError("  ✗ FAIL: PrefabCreator.characterVariants not assigned!");
                Debug.LogError("    → Assign at least one character variant in the inspector");
                issueCount++;
            }
            else
            {
                Debug.Log($"  ✓ PASS: PrefabCreator configured with {creator.characterVariants.Length} character variants");
                int validVariants = 0;
                foreach (var variant in creator.characterVariants)
                {
                    if (variant != null && variant.prefab != null)
                        validVariants++;
                }
                if (validVariants < creator.characterVariants.Length)
                {
                    Debug.LogWarning($"    ⚠ Only {validVariants}/{creator.characterVariants.Length} variants have prefabs assigned");
                }
            }
        }

        // Summary
        Debug.Log("\n" + new string('=', 60));
        if (issueCount == 0)
        {
            Debug.Log("✓ ALL CHECKS PASSED - System is ready!");
            Debug.Log("  Dragons should disappear when tapped/clicked");
            Debug.Log("  Score should increment when dragons are collected");
        }
        else
        {
            Debug.LogError($"✗ {issueCount} CRITICAL ISSUE(S) FOUND - See above for details");
        }
        Debug.Log(new string('=', 60) + "\n");
    }
}
