# Dragon Collection System - Implementation Guide

## Overview

The dragon collection system allows dragons in the GarbageGO AR game to disappear when tapped/clicked, and increments the player's score.

## How It Works

### 1. Dragon Spawning (PrefabCreator.cs)

- When an AR marker image is detected, `PrefabCreator` spawns dragon prefabs
- Each spawned dragon automatically gets the `Collectible` script attached
- Dragons are positioned randomly around the AR marker

### 2. Collection Detection (Collectible.cs)

- **Input Detection:**

  - Mobile (Android/iOS): Detects touch input with `Input.touchCount`
  - Desktop/Editor: Detects mouse clicks with `Input.GetMouseButtonDown(0)`

- **Raycasting:**

  - When input is detected, a raycast is cast from the camera through the screen position
  - Checks if any colliders along the ray match the current dragon
  - Uses `Physics.RaycastAll()` for reliable detection

- **Visual Feedback:**
  - When a dragon is hit, all renderers are immediately disabled
  - Dragon is destroyed after 0.2 seconds delay
  - Score is incremented via `ScoreManager`

### 3. Score Management (ScoreManager.cs)

- Singleton instance tracks the current score
- Updates TextMeshPro UI when `AddMonster()` is called
- Integrates with BadgeManager for achievement checking

## Key Components

### Collectible.cs

```csharp
public class Collectible : MonoBehaviour
{
    public string monsterName = "Garbage Monster";  // Set by prefab or PrefabCreator

    // Automatically creates SphereCollider if missing (radius 5.0)
    // Detects tap/click input
    // Disables renderers and destroys the dragon
}
```

### PrefabCreator.cs

```csharp
public class PrefabCreator : MonoBehaviour
{
    public GameObject monsterPrefab;      // Assign in inspector
    public int monsterCount = 5;          // How many dragons to spawn
    public float spawnRadius = 0.25f;     // Random spawn area

    // Automatically adds Collectible if missing
    // Sets monsterName from prefab name
}
```

### ScoreManager.cs

```csharp
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;  // Singleton
    public int score = 0;

    public void AddMonster(string monsterName)
    {
        score += 1;
        scoreText.text = score.ToString();  // Update UI
    }
}
```

## Setup Checklist

### ✓ Scene Setup

- [ ] `ScoreManager` exists in scene as singleton
- [ ] `ScoreManager.scoreText` is assigned to TextMeshPro UI element
- [ ] Main camera exists in scene (required for raycasting)
- [ ] AR Foundation is configured for image tracking

### ✓ GameObject Setup (if using PrefabCreator)

- [ ] Create empty GameObject with `ARTrackedImageManager` component
- [ ] Add `PrefabCreator` script to same GameObject
- [ ] Assign dragon prefab to `monsterPrefab` field
- [ ] Set `monsterCount` (5-10 recommended)
- [ ] Set `spawnRadius` (0.25 is good for AR)

### ✓ Project Settings

- [ ] Active Input Handling: **New Input System only** (not "Both")
  - File: `ProjectSettings/ProjectSettings.asset`
  - Line 941: `activeInputHandler: 1` (not 2)

### ✓ Dragon Prefabs

- [ ] Should have colliders (BoxCollider or SphereCollider)
- [ ] **DO NOT** need Collectible script pre-attached (added at runtime)
- [ ] Should have visible renderers (SkinnedMeshRenderer, etc.)

## Testing

### In Editor

1. Open scene with dragons/monsters
2. Click on a dragon - it should:
   - Disappear (renderer disabled)
   - Log "[Collectible] 🎉 COLLECTED: ..." in console
   - Increment the score text
3. Check console for debug messages like:
   - `[Collectible] Initialized: ...`
   - `[Collectible] Raycast at ...: X hits`
   - `[Collectible] Hit: ...`

### On Mobile Device

1. Build and deploy to Android/iOS device
2. Point camera at AR marker image
3. Tap on dragon - should disappear and increment score
4. Check logcat/device console for debug output

### Debugging

1. Attach `DragonCollectionTest` script to any GameObject
2. Open console at runtime
3. See diagnostic information:
   - Number of Collectible components found
   - ScoreManager status
   - Collider setup
   - Input system type

## Troubleshooting

### Dragons Don't Disappear

**Symptoms:** Tap/click does nothing
**Solutions:**

1. Check console for "[Collectible]" messages
2. Run `DragonCollectionTest` to diagnose
3. Verify ScoreManager exists: `ScoreManager.instance != null`
4. Verify Main Camera exists
5. Check ProjectSettings.asset `activeInputHandler: 1`

### Score Doesn't Update

**Symptoms:** Dragon disappears but score unchanged
**Solutions:**

1. Check ScoreManager exists in scene
2. Check ScoreManager.scoreText is assigned
3. Check for error: "[Collectible] ScoreManager instance not found!"

### Raycasting Issues

**Symptoms:** Only center/certain dragons respond
**Solutions:**

1. Verify colliders are enabled
2. Check collider size (should be radius 5.0 for dragons)
3. Look for "Raycast at ...: 0 hits" in console

### Mobile Input Not Working

**Symptoms:** Works in editor but not on device
**Solutions:**

1. Change ProjectSettings.asset: `activeInputHandler: 2` → `activeInputHandler: 1`
2. Rebuild and redeploy
3. Check logcat: `adb logcat | grep Collectible`

## Debug Commands

```csharp
// Force collect a dragon in code
Collectible coll = GetComponent<Collectible>();
if (coll != null) coll.CollectMonster();

// Check all dragons in scene
Collectible[] all = FindObjectsOfType<Collectible>();
foreach(var c in all) Debug.Log($"{c.name}: {c.monsterName}");

// Check score
Debug.Log($"Score: {ScoreManager.instance.score}");
```

## Input System Details

### Platform-Specific Behavior

```csharp
// Mobile (Android/iOS)
#if UNITY_ANDROID || UNITY_IOS
    // Uses Input.touchCount and touch.phase
    // Supports multitouch (first touch only)
#endif

// Desktop/Editor (All others)
    // Uses Input.GetMouseButtonDown(0)
    // Supports mouse click
```

### Physics Raycast Configuration

- **Ray Origin:** Camera.main.ScreenPointToRay(screenPos)
- **Ray Length:** 1000 units
- **Collider Type:** Any (not just trigger)
- **Detection:** All colliders along ray (RaycastAll)

## Performance Notes

- Collectible script is lightweight
- Single Update() check for input
- SphereCollider (radius 5) is optimized for dragons
- Destroy() delayed 0.2s to allow animation/particle effects

## Future Improvements

- [ ] Add collection animation/particle effects
- [ ] Add collection sound effects
- [ ] Add collected count per monster type
- [ ] Add difficulty scaling (more dragons as score increases)
- [ ] Add UI feedback (popup on collection)
