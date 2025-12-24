using UnityEngine;

public class CollectDragon : MonoBehaviour
{
    public int points = 1;

    void OnMouseDown()
    {
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.AddScore(points);
        }

        Destroy(gameObject);
    }
}
