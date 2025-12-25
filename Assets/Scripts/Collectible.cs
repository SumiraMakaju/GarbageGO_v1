using UnityEngine;
using UnityEngine.EventSystems;

public class Collectible : MonoBehaviour, IPointerClickHandler
{
    public string monsterName = "Garbage Monster";

    public void OnPointerClick(PointerEventData eventData)
    {
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.AddMonster(monsterName);
        }
        Destroy(gameObject);
    }
}

