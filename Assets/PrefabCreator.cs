using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PrefabCreator : MonoBehaviour
{
    [SerializeField] private GameObject dragonPrefab;
    [SerializeField] private Vector3 prefabOffset;
    private GameObject dragon;
    private ARTrackedImageManager aRTrackedImageManager;

    private void OnEnable()
    {
        aRTrackedImageManager = gameObject.GetComponent<ARTrackedImageManager>();
        aRTrackedImageManager.trackedImagesChanged += OnImageChanged;
    }

    private void OnImageChanged(ARTrackedImagesChangedEventArgs obj)
    {
        foreach (ARTrackedImage image in obj.added)
        {
            // if (trackedImage.referenceImage.name == "dragon_image")
            // {
                dragon = Instantiate(dragonPrefab, image.transform);
                dragon.transform.position += prefabOffset;
            // }
        }

        // foreach (var trackedImage in eventArgs.updated)
        // {
        //     if (trackedImage.referenceImage.name == "dragon_image" && dragon != null)
        //     {
        //         dragon.transform.position = trackedImage.transform.position + prefabOffset;
        //     }
        // }

        // foreach (var trackedImage in eventArgs.removed)
        // {
        //     if (trackedImage.referenceImage.name == "dragon_image" && dragon != null)
        //     {
        //         Destroy(dragon);
        //     }
        // }

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created

}
