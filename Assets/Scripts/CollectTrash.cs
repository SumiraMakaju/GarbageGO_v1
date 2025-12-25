using UnityEngine;

public class CollectTrash : MonoBehaviour
{

    public string monsterName;

    void OnMouseDown()
    {
        ScoreManager.instance.AddMonster(monsterName);
        Destroy(gameObject);
    }
}

