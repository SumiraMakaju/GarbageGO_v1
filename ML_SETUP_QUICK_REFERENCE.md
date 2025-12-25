# ML Model Integration - Quick Reference Card

## 🚀 Fast Setup (5 Minutes)

### Step 1: Install Barracuda

```
Window → Package Manager → Add (+)
→ Add package by name → com.unity.barracuda → Add
```

### Step 2: Create MLSystem GameObject

- Create empty GameObject
- Name it: **MLSystem**
- Don't move it

### Step 3: Add Components

Select MLSystem, then "Add Component":

- [ ] BarracudaMLModel
- [ ] TrashDetectionAPI
- [ ] MLTrashDetectionManager

### Step 4: Configure BarracudaMLModel

```
Model Path: Models/waste_classifier
Input Width: 224
Input Height: 224
Confidence Threshold: 0.6
Use GPU: true
```

### Step 5: Configure MLTrashDetectionManager

Add Trash Config mappings for each trash type:

```
[0] trash_type: plastic_bottle → Dragon Prefab
[1] trash_type: plastic_bag → Dragon Prefab
[2] trash_type: can → Dragon Prefab
[3] trash_type: metal_waste → Dragon Prefab
[4] trash_type: paper → Dragon Prefab
[5] trash_type: cardboard → Dragon Prefab
[6] trash_type: glass → Dragon Prefab
[7] trash_type: organic → Dragon Prefab
```

### Step 6: Press Play

Check Console for: ✓ Model loaded successfully

---

## 📊 File Locations

```
waste_classifier.onnx → Assets/Models/
BarracudaMLModel.cs → Assets/Scripts/
TrashDetectionAPI.cs → Assets/Scripts/
MLTrashDetectionManager.cs → Assets/Scripts/
```

---

## 🧪 Quick Tests

### Test 1: Model Loading

1. Add MLSetupVerifier to any GameObject
2. Press Play
3. Check Console for ✓ signs

### Test 2: Inference

1. Add MLModelTester to scene
2. Press Play
3. Check Console for detection results

---

## ⚙️ Inspector Settings Reference

### BarracudaMLModel

| Setting              | Value                     |
| -------------------- | ------------------------- |
| Model Path           | `Models/waste_classifier` |
| Input Width          | 224                       |
| Input Height         | 224                       |
| Confidence Threshold | 0.6                       |
| Use GPU              | true                      |

### TrashDetectionAPI

| Setting              | Value   |
| -------------------- | ------- |
| Use Local Model      | true    |
| Confidence Threshold | 0.6     |
| API URL              | (empty) |

### MLTrashDetectionManager

| Setting                 | Value                |
| ----------------------- | -------------------- |
| Detection Interval      | 2.0                  |
| Max Trash Per Detection | 3                    |
| Trash Configs           | (see mappings above) |

---

## 🔍 Debugging Checklist

- [ ] Model file exists: `Assets/Models/waste_classifier.onnx`
- [ ] Barracuda installed: `Window → Package Manager`
- [ ] MLSystem GameObject created
- [ ] All 3 components added
- [ ] Model Path set to: `Models/waste_classifier`
- [ ] Console shows: ✓ Model loaded
- [ ] Trash configs mapped correctly
- [ ] Prefabs assigned to trash types

---

## ❌ Common Issues

| Problem            | Fix                                               |
| ------------------ | ------------------------------------------------- |
| Model not found    | Check path: `Assets/Models/waste_classifier.onnx` |
| Barracuda error    | Install: `com.unity.barracuda`                    |
| "Model not loaded" | Verify Model Path in Inspector                    |
| GPU error          | Set Use GPU to `false`                            |
| No detections      | Check Confidence Threshold (0.6)                  |
| Trash won't spawn  | Verify trash configs are mapped                   |

---

## 📱 Performance Settings

### For Slow Devices

```
Detection Interval: 3.0
Max Trash: 1
Use GPU: false
Input Width/Height: 128
```

### Balanced (Default)

```
Detection Interval: 2.0
Max Trash: 3
Use GPU: true
Input Width/Height: 224
```

### High Quality

```
Detection Interval: 1.0
Max Trash: 5
Use GPU: true
Input Width/Height: 512
```

---

## 🎮 Gameplay Integration

✅ **Already Works:**

- Portal suction animation (when tapping)
- +1 floating text animation
- Score tracking & saving
- Collection UI
- Dragon database

✅ **ML Adds:**

- Automatic trash detection
- Dynamic spawning
- Real-world camera input

---

## 📞 Console Output Reference

### Good Signs ✓

```
[BarracudaMLModel] ✓ Model loaded: waste_classifier
[BarracudaMLModel] ✓ Worker created successfully
[TrashDetectionAPI] Inference complete: 2 detections
[MLTrashDetectionManager] Spawned 2 trash items
```

### Bad Signs ✗

```
[BarracudaMLModel] ❌ Failed to load model
[BarracudaMLModel] ❌ Barracuda package not installed
[TrashDetectionAPI] Model is not loaded
```

---

## 🚦 Go/No-Go Checklist

| Item                    | Status |
| ----------------------- | ------ |
| Barracuda installed     | [ ]    |
| Model file exists       | [ ]    |
| MLSystem created        | [ ]    |
| Components added        | [ ]    |
| Model Path configured   | [ ]    |
| Trash configs mapped    | [ ]    |
| Prefabs assigned        | [ ]    |
| Console: ✓ Model loaded | [ ]    |
| Trash spawns in scene   | [ ]    |
| Collection works        | [ ]    |

**All checked? You're ready to play! 🎉**

---

## 🎯 What Happens When You Press Play

1. **BarracudaMLModel** loads waste_classifier.onnx
2. **MLTrashDetectionManager** starts detection loop
3. Every 2 seconds: Camera frame → Inference
4. Detections above 0.6 threshold → Trash spawns
5. Player taps trash → Portal animation
6. +1 floats → Score updates
7. Data saved to PlayerPrefs

---

## 📚 Full Documentation

- `ML_SETUP_GUIDE.md` - Detailed instructions
- `ML_SETUP_CHECKLIST.md` - Complete checklist
- `ML_INTEGRATION_SUMMARY.md` - Full overview
- `ML_SETUP_QUICK_REFERENCE.md` - This file

---

## 💡 Tips & Tricks

### Faster Setup

- Copy this config exactly into Inspector
- Use default trash type mappings
- Don't change Input Width/Height

### Better Performance

- Increase Detection Interval to 3-4 seconds
- Reduce Max Trash to 1-2
- Set Use GPU to false if errors occur

### Better Accuracy

- Lower Confidence Threshold to 0.4-0.5
- Increase Detection Interval (more time to detect)
- Check model was trained correctly

---

## 🎓 Learning Path

1. **Read**: `ML_SETUP_GUIDE.md` (10 min)
2. **Setup**: Follow quick steps above (5 min)
3. **Test**: Run MLSetupVerifier (1 min)
4. **Play**: Try with camera (5 min)
5. **Customize**: Adjust settings for your game

---

**Total setup time: ~30 minutes** ⏱️

---

_Last updated: December 2025_
_For issues, check console output and troubleshooting guide_
