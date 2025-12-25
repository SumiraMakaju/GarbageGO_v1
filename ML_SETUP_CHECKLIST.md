# ML Model Setup Checklist

## ✅ Pre-Flight Checks

### Model File

- [x] File exists: `Assets/Models/waste_classifier.onnx`
- [x] .meta file exists: `Assets/Models/waste_classifier.onnx.meta`
- [ ] Model is imported as "Neural Network Model" (right-click → properties)

### Scripts Created

- [x] `BarracudaMLModel.cs` - Model loading and inference
- [x] `TrashDetectionAPI.cs` - Detection API wrapper (updated)
- [x] `MLTrashDetectionManager.cs` - Auto-detection manager
- [x] `MLSetupVerifier.cs` - Setup verification script
- [x] `MLModelTester.cs` - Quick testing script

### Project Structure

```
Assets/
├── Models/
│   ├── waste_classifier.onnx ✓
│   └── waste_classifier.onnx.meta ✓
├── Resources/
│   └── (Badges folder)
└── Scripts/
    ├── BarracudaMLModel.cs ✓
    ├── TrashDetectionAPI.cs ✓
    ├── MLTrashDetectionManager.cs ✓
    ├── MLSetupVerifier.cs ✓
    ├── MLModelTester.cs ✓
    └── ... (other scripts)
```

## 🔧 Setup Steps

### Step 1: Install Barracuda

```
Window → Package Manager
→ Add (+) → Add package by name
→ Type: com.unity.barracuda
→ Click Add
```

**Estimated time: 2-5 minutes**

### Step 2: Create MLSystem GameObject

1. In your main scene, create empty GameObject
2. Name it: **"MLSystem"**
3. Position: Anywhere (usually 0, 0, 0)

### Step 3: Add Components

With MLSystem selected, add these via "Add Component":

- [ ] `BarracudaMLModel`
- [ ] `TrashDetectionAPI`
- [ ] `MLTrashDetectionManager`

### Step 4: Configure BarracudaMLModel

In Inspector for BarracudaMLModel:

```
Model Path: Models/waste_classifier
Input Width: 224
Input Height: 224
Confidence Threshold: 0.6
Use GPU: true
```

### Step 5: Configure TrashDetectionAPI

```
Use Local Model: true
Confidence Threshold: 0.6
API URL: (empty if using local)
```

### Step 6: Configure MLTrashDetectionManager

```
Detection Interval: 2.0
Max Trash Per Detection: 3
Trash Configs: (add mappings below)
```

### Trash Config Mappings

Add these to the Trash Configs list:

```
[0] trash_type: "plastic_bottle"
    prefab: (assign corresponding dragon prefab)

[1] trash_type: "plastic_bag"
    prefab: (assign corresponding dragon prefab)

[2] trash_type: "can"
    prefab: (assign corresponding dragon prefab)

... (add all 8 trash types)
```

## 🧪 Verification Steps

### Quick Test 1: Model Loading

1. Create empty GameObject: **"Tester"**
2. Add `MLSetupVerifier.cs` component
3. **Press Play**
4. Check Console for setup verification

**Expected output:**

```
✓ Model file found: waste_classifier.onnx
✓ Barracuda package is installed
✓ BarracudaMLModel found in scene
✓ Model is loaded and ready
```

### Quick Test 2: Inference Test

1. Add `MLModelTester.cs` to your scene
2. **Press Play**
3. Check Console for inference test results

**Expected output:**

```
✓ SUCCESS: Model loaded via Resources
✓ Inference successful!
  Detections: X
```

## 🐛 Common Issues & Fixes

### Issue 1: "Model file NOT found"

**Cause**: File in wrong location or not imported
**Fix**:

1. Check: `Assets/Models/waste_classifier.onnx` exists
2. Right-click model → "Reimport"
3. Check it's imported as "Neural Network Model"

### Issue 2: "Barracuda not installed"

**Cause**: Package not added
**Fix**:

```
Window → Package Manager
→ Add (+) → Add package by name
→ com.unity.barracuda
```

### Issue 3: "Model is not loaded"

**Cause**: Path mismatch or loading error
**Fix**:

1. In BarracudaMLModel Inspector: set Model Path to **exactly**: `Models/waste_classifier`
2. Restart Unity editor
3. Check Console for detailed error

### Issue 4: GPU Backend Error

**Cause**: GPU not compatible
**Fix**:

1. In BarracudaMLModel: set **Use GPU** to `false`
2. This uses CPU inference (slower but compatible)

## 📊 Expected Console Output

### On Scene Play:

```
[BarracudaMLModel] ✓ Model loaded: waste_classifier
[BarracudaMLModel] Input shape: 224x224
[BarracudaMLModel] Model inputs: input_name
[BarracudaMLModel] Model outputs: output_name
[BarracudaMLModel] ✓ Worker created successfully
[TrashDetectionAPI] Barracuda inference: X detections found
[MLTrashDetectionManager] Spawned X trash items from detection
```

## 🎮 Gameplay Integration

Once setup is complete:

1. **Inference runs** automatically every 2 seconds
2. **Detections appear** as trash in the scene
3. **Players tap** trash to collect
4. **Portal animation** plays (already implemented)
5. **+1 text floats** up (already implemented)
6. **Score updates** with points
7. **Data saves** via PlayerPrefs

## 📱 Performance Tips

- **Detection Interval**: Increase from 2.0 to 3.0+ for better performance
- **Max Trash**: Reduce from 3 to 1-2 on low-end devices
- **Use GPU**: Set to false for consistent performance
- **Input Size**: Smaller = faster (but less accurate)

## 🚀 What's Next

After successful setup:

1. Fine-tune detection interval
2. Adjust trash spawn locations
3. Add sound effects on detection
4. Create particle effects
5. Add difficulty progression
6. Implement leaderboards

## 📞 Debug Help

If something doesn't work:

1. Run **MLSetupVerifier** script
2. Check Console for detailed error messages
3. Verify all components are on **MLSystem** GameObject
4. Restart Unity Editor
5. Rebuild/Reimport model file

## ✨ Success Indicators

✓ All setup steps completed
✓ Console shows successful model loading
✓ No red error messages in Console
✓ Inference test shows detections
✓ Trash spawns in scene
✓ Collection works with animations
✓ Score updates correctly

**You're ready to play!** 🎉
