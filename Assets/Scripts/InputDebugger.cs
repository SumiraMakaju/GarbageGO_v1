using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Debug script to verify input detection and raycasting
/// Add this to a GameObject in your scene to enable input debugging
/// </summary>
public class InputDebugger : MonoBehaviour
{
    void Update()
    {
        // Log touch input - New Input System
        #if UNITY_ANDROID || UNITY_IOS
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            Debug.Log($"[InputDebugger] Touch detected at: {touchPosition}");
            
            if (Camera.main != null)
            {
                Ray ray = Camera.main.ScreenPointToRay(touchPosition);
                RaycastHit[] hits = Physics.RaycastAll(ray, 1000f);
                
                Debug.Log($"[InputDebugger] Raycast hit {hits.Length} objects");
                foreach (RaycastHit hit in hits)
                {
                    Debug.Log($"  - {hit.collider.gameObject.name} (tag: {hit.collider.gameObject.tag})");
                }
            }
        }
        #endif

        // Log mouse input - New Input System
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Debug.Log($"[InputDebugger] Mouse click detected at: {mousePosition}");
            
            if (Camera.main != null)
            {
                Ray ray = Camera.main.ScreenPointToRay(mousePosition);
                RaycastHit[] hits = Physics.RaycastAll(ray, 1000f);
                
                Debug.Log($"[InputDebugger] Raycast hit {hits.Length} objects");
                foreach (RaycastHit hit in hits)
                {
                    Debug.Log($"  - {hit.collider.gameObject.name} (tag: {hit.collider.gameObject.tag})");
                }
            }
        }
    }
}
