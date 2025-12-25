using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PrefabCreator : MonoBehaviour
{
    [System.Serializable]
    public class CharacterVariant
    {
        public string characterName;
        public GameObject prefab;
    }

    public CharacterVariant[] characterVariants;
    public int monsterCount = 10;
    public float spawnRadius = 0.5f;
    public bool manualSpawnMode = false;  // Set to true for testing without AR

    private ARTrackedImageManager imageManager;
    private bool spawned = false;

    void Awake()
    {
        imageManager = GetComponent<ARTrackedImageManager>();
        
        if (characterVariants == null || characterVariants.Length == 0)
        {
            Debug.LogWarning("PrefabCreator: No character variants assigned in inspector!");
        }
    }

    void OnEnable()
    {
        if (imageManager != null && !manualSpawnMode)
        {
            try
            {
                imageManager.trackablesChanged.AddListener(OnImagesChanged);
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"PrefabCreator: AR Foundation error on enable - {ex.Message}");
            }
        }
    }

    void OnDisable()
    {
        if (imageManager != null && !manualSpawnMode)
        {
            try
            {
                imageManager.trackablesChanged.RemoveListener(OnImagesChanged);
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"PrefabCreator: AR Foundation error on disable - {ex.Message}");
            }
        }
    }

    void Update()
    {
        // Manual spawn mode for testing (press SPACE to spawn)
        if (manualSpawnMode && !spawned && Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Debug.Log("PrefabCreator: Manual spawn triggered!");
            SpawnMonsters(transform);
            spawned = true;
        }
    }

    void OnImagesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> args)
    {
        try
        {
            foreach (var image in args.added)
            {
                if (!spawned && image.trackingState == TrackingState.Tracking)
                {
                    Debug.Log($"PrefabCreator: AR image detected - spawning at {image.transform.position}");
                    SpawnMonsters(image.transform);
                    spawned = true;
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"PrefabCreator: Error during image detection - {ex.Message}");
        }
    }

    void SpawnMonsters(Transform parent)
    {
        if (characterVariants == null || characterVariants.Length == 0)
        {
            Debug.LogError("PrefabCreator: No character variants available - Cannot spawn monsters!");
            return;
        }

        Debug.Log($"PrefabCreator: Spawning {monsterCount} monsters with {characterVariants.Length} variants");

        for (int i = 0; i < monsterCount; i++)
        {
            // Select random character variant
            CharacterVariant variant = characterVariants[Random.Range(0, characterVariants.Length)];
            
            if (variant.prefab == null)
            {
                Debug.LogWarning($"PrefabCreator: Character variant '{variant.characterName}' has no prefab assigned!");
                continue;
            }

            // Random spawn position within spawnRadius
            Vector3 randomPos = parent.position + new Vector3(
                Random.Range(-spawnRadius, spawnRadius),
                0,
                Random.Range(-spawnRadius, spawnRadius)
            );

            GameObject spawnedMonster = Instantiate(variant.prefab, randomPos, Quaternion.identity, parent);
            spawnedMonster.name = $"{variant.characterName}_{i + 1}";
            
            // Scale down to match the initial dragon size (0.015)
            spawnedMonster.transform.localScale = new Vector3(0.015f, 0.015f, 0.015f);
            
            // Fix the character in place - disable physics movement
            Rigidbody rb = spawnedMonster.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }
            
            // Disable animator to prevent animation from affecting position
            Animator animator = spawnedMonster.GetComponent<Animator>();
            if (animator != null)
            {
                animator.enabled = false;
            }
            
            // Ensure the spawned character has the Collectible script
            Collectible collectible = spawnedMonster.GetComponent<Collectible>();
            if (collectible == null)
            {
                collectible = spawnedMonster.AddComponent<Collectible>();
            }
            
            // Set the monster name from variant
            collectible.monsterName = variant.characterName;

            Debug.Log($"[{i + 1}/{monsterCount}] Spawned {variant.characterName} at {randomPos} (scale: 0.015) - FIXED at location!");
        }

        Debug.Log($"PrefabCreator: Finished spawning {monsterCount} monsters at trash image location");
    }
}
