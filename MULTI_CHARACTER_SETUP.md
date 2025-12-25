# Multi-Character Spawning Setup Guide

## Quick Setup (5 Minutes)

### Step 1: Find PrefabCreator GameObject

1. Open your scene in Unity
2. Find the GameObject with **ARTrackedImageManager** component
3. It should also have **PrefabCreator** script

### Step 2: Assign Character Variants

1. Select the PrefabCreator GameObject
2. In Inspector, expand **PrefabCreator** script
3. Find **Character Variants** array
4. Set array size to **8** (or more)
5. Drag prefabs from `Assets/FourEvilDragonsHP/Prefab/`:
   - Element 0: `DragonNightmare/Blue.prefab`
   - Element 1: `DragonNightmare/Green.prefab`
   - Element 2: `DragonSoulEater/Blue.prefab`
   - Element 3: `DragonSoulEater/Red.prefab`
   - Element 4: `DragonTerrorBringer/Blue.prefab`
   - Element 5: `DragonTerrorBringer/Purple.prefab`
   - Element 6: `DragonUsurper/Blue.prefab`
   - Element 7: `DragonUsurper/Green.prefab`

### Step 3: Configure Spawn Settings

- **Monster Count**: 10-15 (number of characters to spawn)
- **Spawn Radius**: 0.5 (spread area around image)
- **Manual Spawn Mode**: FALSE (for AR) or TRUE (for testing)

### Step 4: Test in Editor

If using **Manual Spawn Mode = TRUE**:

1. Play the scene
2. Check Console for messages
3. Press **SPACE** to spawn characters
4. Characters should appear around the GameObject

If using **Manual Spawn Mode = FALSE** (AR Mode):

1. Run on device with trash image
2. Point camera at image
3. Characters should spawn automatically

## Troubleshooting

### No characters visible?

1. Open Console (Window > General > Console)
2. Attach **PrefabCreatorDebug** script to any GameObject
3. Check debug messages showing:
   - ✓ PrefabCreator found
   - ✓ Character variants count
   - ✓ Valid prefabs assigned
   - ✗ Any warnings about NULL prefabs

### "No valid variants" warning?

- You haven't assigned prefabs to the array
- Go back to Step 2 and drag prefabs

### Only 1 character spawning?

- Check **monsterCount** value (should be > 1)
- Check Console for error messages
- Each iteration should log: `[1/10] Spawned DragonXXX...`

### Characters not interactable?

- Verify Collectible script is attached (added automatically)
- Check if colliders are enabled
- Try tapping/clicking on them

## Manual Testing Mode

If you want to test without AR image:

1. Set **Manual Spawn Mode** to TRUE
2. Play scene
3. Press **SPACE** to spawn

For forced spawn (debug):

- Alt + S to trigger spawn with reflection

## What Each Component Does

- **PrefabCreator**: Manages spawning of characters
- **CharacterVariant**: References character prefab + name
- **characterVariants[]**: Array of all available characters
- **monsterCount**: How many to spawn per trigger
- **spawnRadius**: Random area to place them
- **manualSpawnMode**: FALSE=AR, TRUE=keyboard testing

## Expected Output in Console

```
PrefabCreator: Spawning 10 monsters with 8 variants
[1/10] Spawned DragonNightmare_Blue at (0.2, 0, 0.1) - FIXED at location!
[2/10] Spawned DragonSoulEater_Red at (-0.3, 0, 0.2) - FIXED at location!
[3/10] Spawned DragonTerrorBringer_Purple at (0.1, 0, -0.2) - FIXED at location!
...
[10/10] Spawned DragonUsurper_Green at (-0.2, 0, 0.3) - FIXED at location!
PrefabCreator: Finished spawning 10 monsters at trash image location
```

Each character should:

- ✓ Be visible in scene
- ✓ Stay at spawn location
- ✓ Disappear when tapped
- ✓ Increment score when collected
