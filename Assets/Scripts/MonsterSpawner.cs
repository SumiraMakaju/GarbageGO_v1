using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
   public static MonsterSpawner Instance;

    [Header("Spawn Settings")]
    public GameObject[] monsterPrefabs;
    public int maxMonsters = 5;
    public float spawnInterval = 15f;
    public float spawnDistance = 2f;
    public float spawnRadius = 1.2f;

    private int currentMonsters = 0;
    internal static object instance;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        InvokeRepeating(nameof(SpawnMonster), 1f, spawnInterval);
    }

    void SpawnMonster()
    {
        if (currentMonsters >= maxMonsters)
            return;

        if (Camera.main == null)
            return;

        Vector3 camPos = Camera.main.transform.position;
        Vector3 camForward = Camera.main.transform.forward;

        Vector3 randomOffset = new Vector3(
            Random.Range(-spawnRadius, spawnRadius),
            Random.Range(-0.2f, 0.2f),
            Random.Range(-spawnRadius, spawnRadius)
        );

        Vector3 spawnPos = camPos + camForward * spawnDistance + randomOffset;

        GameObject prefab =
            monsterPrefabs[Random.Range(0, monsterPrefabs.Length)];

        GameObject monster =
            Instantiate(prefab, spawnPos, Quaternion.identity);

        currentMonsters++;
    }

    public void MonsterCollected()
    {
        currentMonsters = Mathf.Max(0, currentMonsters - 1);
    }
}
