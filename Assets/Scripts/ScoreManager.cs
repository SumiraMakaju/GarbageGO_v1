using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public int score = 0;
    public TextMeshProUGUI scoreText;

    public List<string> collectedMonsters = new List<string>();

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void AddMonster(string monsterName)
    {
        score += 1;
        collectedMonsters.Add(monsterName);
        scoreText.text = score.ToString();

        BadgeManager.instance?.CheckBadge(score);
    }
}
