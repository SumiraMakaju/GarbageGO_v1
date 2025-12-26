//using UnityEngine;
//using UnityEngine.InputSystem;
//using TMPro;
//using UnityEngine.UI;
//using System.Collections;

//public class Collectible : MonoBehaviour
//{
//    // ✅ MOVE THIS INSIDE THE CLASS
//    public MonsterType monsterType;

//    public string monsterName = "Garbage Monster";
//    private Collider objectCollider;
//    private bool isDestroyed = false;
//    private Animator animator;
//    private Vector3 spawnPosition;
//    private Rigidbody rb;

//    void Start()
//    {
//        spawnPosition = transform.position;

//        objectCollider = GetComponent<Collider>();
//        if (objectCollider == null)
//        {
//            SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
//            sphereCollider.radius = 5f;
//            objectCollider = sphereCollider;
//        }

//        objectCollider.isTrigger = false;

//        rb = GetComponent<Rigidbody>();
//        if (rb != null)
//        {
//            rb.isKinematic = true;
//            rb.constraints = RigidbodyConstraints.FreezeAll;
//            rb.useGravity = false;
//        }

//        animator = GetComponent<Animator>();
//    }

//    void Update()
//    {
//        if (isDestroyed) return;

//        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
//        {
//            HandleInput(Mouse.current.position.ReadValue());
//        }
//    }

//    void HandleInput(Vector2 screenPosition)
//    {
//        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
//        if (Physics.Raycast(ray, out RaycastHit hit))
//        {
//            if (hit.collider.gameObject == gameObject)
//            {
//                CollectMonster();
//            }
//        }
//    }

//    void CollectMonster()
//    {
//        if (isDestroyed) return;
//        isDestroyed = true;

//        objectCollider.enabled = false;

//        if (ScoreManager.instance != null)
//        {
//            ScoreManager.instance.AddMonster(monsterType.ToString());
//        }

//        if (MonsterSpawner.Instance != null)
//        {
//            MonsterSpawner.Instance.MonsterCollected();
//        }


//    }


//}
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Collectible : MonoBehaviour
{
    public MonsterType monsterType;
    public string monsterName = "Garbage Monster";

    private Collider objectCollider;
    private bool isDestroyed = false;

    void Awake()
    {
        // Ensure collider exists
        objectCollider = GetComponent<Collider>();
        if (objectCollider == null)
        {
            SphereCollider col = gameObject.AddComponent<SphereCollider>();
            col.radius = 0.25f; // ✅ realistic AR size
            col.center = Vector3.zero;
            objectCollider = col;
        }

        objectCollider.isTrigger = false;

        // Ensure raycastable layer
        gameObject.layer = LayerMask.NameToLayer("Default");
    }

    void Update()
    {
        if (isDestroyed) return;

        // Mouse (Editor)
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryCollect(Mouse.current.position.ReadValue());
        }

        // Touch (Mobile AR)
        if (Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            TryCollect(Touchscreen.current.primaryTouch.position.ReadValue());
        }
    }

    void TryCollect(Vector2 screenPos)
    {
        if (Camera.main == null)
        {
            Debug.LogError("❌ No Main Camera found");
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            Debug.Log("🎯 Hit: " + hit.collider.name);

            if (hit.collider == objectCollider)
            {
                CollectMonster();
            }
        }
    }

    void CollectMonster()
    {
        if (isDestroyed) return;
        isDestroyed = true;

        objectCollider.enabled = false;

        // SCORE
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.AddMonster(monsterType.ToString());
        }

        // SPAWNER
        if (MonsterSpawner.Instance != null)
        {
            MonsterSpawner.Instance.MonsterCollected();
        }

        // SIMPLE COLLECT ANIMATION
        StartCoroutine(CollectAnimation());
    }

    IEnumerator CollectAnimation()
    {
        Vector3 startScale = transform.localScale;
        float t = 0f;

        while (t < 0.35f)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t / 0.35f);
            transform.Rotate(Vector3.up, 360f * Time.deltaTime);
            yield return null;
        }

        Destroy(gameObject);
    }
}


