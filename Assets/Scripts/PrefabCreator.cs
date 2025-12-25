using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PrefabCreator : MonoBehaviour
{
    public GameObject monsterPrefab;
    public int monsterCount = 5;
    public float spawnRadius = 0.25f;

    private ARTrackedImageManager imageManager;
    private bool spawned = false;

    void Awake()
    {
        imageManager = GetComponent<ARTrackedImageManager>();
    }

    void OnEnable()
    {
        imageManager.trackablesChanged.AddListener(OnImagesChanged);
    }

    void OnDisable()
    {
        imageManager.trackablesChanged.RemoveListener(OnImagesChanged);
    }

    void OnImagesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> args)
    {
        foreach (var image in args.added)
        {
            if (!spawned && image.trackingState == TrackingState.Tracking)
            {
                SpawnMonsters(image.transform);
                spawned = true;
            }
        }
    }

    void SpawnMonsters(Transform parent)
    {
        for (int i = 0; i < monsterCount; i++)
        {
            Vector3 randomPos =
                parent.position +
                new Vector3(
                    Random.Range(-spawnRadius, spawnRadius),
                    0,
                    Random.Range(-spawnRadius, spawnRadius)
                );

            Instantiate(monsterPrefab, randomPos, Quaternion.identity, parent);
        }
    }
}
