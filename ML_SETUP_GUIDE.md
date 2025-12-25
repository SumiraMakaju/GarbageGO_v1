# ML Model Integration Setup Guide

## ✅ Current Status

- ✓ Model file: `waste_classifier.onnx` is in `Assets/Models/`
- ✓ Scripts created: BarracudaMLModel, TrashDetectionAPI, MLTrashDetectionManager
- ✓ Setup verifier added: MLSetupVerifier

## 📋 Required Steps

### Step 1: Install Barracuda Package

1. Open Unity Editor
2. Go to **Window → Package Manager**
3. Click the **"+"** button in top-left
4. Select **"Add package by name..."**
5. Type: `com.unity.barracuda`
6. Click **"Add"**

### Step 2: Create ML System in Scene

1. In your main scene, create a new empty GameObject
2. Name it **"MLSystem"**
3. Add these components via **Add Component**:
   - `BarracudaMLModel`
   - `TrashDetectionAPI`
   - `MLTrashDetectionManager`

### Step 3: Configure BarracudaMLModel

In Inspector, set:

- **Model Path**: `Models/waste_classifier`
- **Input Width**: `224` (or your model's input width)
- **Input Height**: `224` (or your model's input height)
- **Confidence Threshold**: `0.6`
- **Use GPU**: `true` (if available)

### Step 4: Configure TrashDetectionAPI

In Inspector, set:

- **Use Local Model**: `true`
- **Confidence Threshold**: `0.6`
- **API URL**: (leave empty if using local model)

### Step 5: Configure MLTrashDetectionManager

In Inspector, set:

- **AR Camera Manager**: (optional, auto-finds if not set)
- **Detection Interval**: `2.0` (seconds between detections)
- **Max Trash Per Detection**: `3`
- **Trash Configs**: Create entries for each trash type:
  - Name: trash type (e.g., "plastic_bottle")
  - Prefab: corresponding dragon prefab

### Step 6: Add Verifier to Scene (Optional but Recommended)

1. Create empty GameObject: "Verifier"
2. Add `MLSetupVerifier.cs` component
3. Press Play to see setup verification in console

## 🧪 Testing

### Test 1: Console Verification

1. Press **Play** in Unity
2. Check Console for messages like:
   ```
   ✓ Model file found: waste_classifier.onnx
   ✓ Barracuda package is installed
   ✓ BarracudaMLModel found in scene
   ✓ Model is loaded and ready
   ```

### Test 2: Manual Inference

Add this test script to a button click:

```csharp
void TestInference()
{
    Texture2D testImage = GetComponent<Renderer>().material.mainTexture as Texture2D;
    if (testImage != null && BarracudaMLModel.instance != null)
    {
        DetectionResponse response = BarracudaMLModel.instance.RunInference(testImage);
        Debug.Log($"Detections: {response.detections.Count}");
    }
}
```

## 🔧 Troubleshooting

### Issue: "Model file NOT found"

- **Solution**: Ensure `waste_classifier.onnx` is in `Assets/Models/`
- Check that the .meta file exists alongside it
- Try: Delete the asset and re-import it

### Issue: "Barracuda package NOT installed"

- **Solution**: Follow **Step 1** above
- If still fails, check your Unity version (Barracuda requires 2021.2+)

### Issue: "Model is not loaded"

- **Solution**: Check Console for specific error messages
- Verify model path matches: `Models/waste_classifier`
- Try building/importing the ONNX model again

### Issue: GPU Backend Error

- **Solution**: Set **Use GPU** to `false` in BarracudaMLModel
- This will use CPU inference (slower but more compatible)

## 📊 Model Output Format

Your `waste_classifier.onnx` model should output:

- **Shape**: [1, num_classes] for classification
- **Type**: Float32
- **Values**: Class probabilities (0-1)

Common class indices:

```
0: plastic_bottle
1: plastic_bag
2: can
3: metal_waste
4: paper
5: cardboard
6: glass
7: organic
```

If your model outputs differently, edit `ParseOutput()` in `BarracudaMLModel.cs`.

## 🎯 What Happens Next

1. **Inference Runs** every 2 seconds (configurable)
2. **Detections** are processed and trash spawns
3. **Collectible** components handle collection
4. **Score** updates with points
5. **+1 Animation** shows floating text

## 📁 File Locations

```
Assets/
├── Models/
│   └── waste_classifier.onnx ✓
├── Resources/
│   └── (Badges folder)
└── Scripts/
    ├── BarracudaMLModel.cs ✓
    ├── TrashDetectionAPI.cs ✓
    ├── MLTrashDetectionManager.cs ✓
    ├── MLSetupVerifier.cs ✓
    ├── Collectible.cs (updated)
    └── ... (other scripts)
```

## 🚀 Next Steps

1. **Install Barracuda** (if not done)
2. **Create MLSystem** in your scene
3. **Configure components** with correct settings
4. **Press Play** and check console
5. **Test with camera** or mock images

Good luck! 🎮
