//using UnityEngine;
//using TMPro;
//using System.Collections.Generic;

//public class ScoreManager : MonoBehaviour
//{
//    public static ScoreManager instance;

//    public int score = 0;

//    // ✅ THESE WERE MISSING (causing your error)
//    public List<string> collectedMonsters = new List<string>();
//    public List<int> collectedPoints = new List<int>();

//    [Header("UI")]
//    public TextMeshProUGUI scoreText;

//    void Awake()
//    {
//        if (instance == null)
//            instance = this;
//        else
//            Destroy(gameObject);
//    }

//    void Start()
//    {
//        UpdateUI();
//    }

//    public void AddMonster(string monsterName)
//    {
//        int pointsEarned = 1; // default

//        if (BadgeManager.instance != null)
//        {
//            pointsEarned = BadgeManager.instance.GetCharacterPoints(monsterName);
//        }

//        score += pointsEarned;
//        collectedMonsters.Add(monsterName);
//        collectedPoints.Add(pointsEarned);

//        Debug.Log($"[ScoreManager] +{pointsEarned} | Total Score: {score}");

//        UpdateUI();

//        if (BadgeManager.instance != null)
//        {
//            BadgeManager.instance.CheckBadge(score);
//        }

//        SaveScore();
//    }

//    void UpdateUI()
//    {
//        if (scoreText != null)
//            scoreText.text = "Score: " + score;
//    }

//    void SaveScore()
//    {
//        PlayerPrefs.SetInt("SCORE", score);
//        PlayerPrefs.Save();
//    }
//}
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    [Header("Score")]
    public int score = 0;

    // ✅ REQUIRED for CollectionUI
    public List<string> collectedMonsters = new List<string>();
    public List<int> collectedPoints = new List<int>();

    [Header("UI")]
    public TextMeshProUGUI scoreText;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        LoadScore();
        UpdateUI();
    }

    // 🔥 Called when monster is collected
    public void AddMonster(string monsterName)
    {
        int pointsEarned = 1;

        // Optional badge logic (safe if BadgeManager missing)
        if (BadgeManager.instance != null)
        {
            pointsEarned = BadgeManager.instance.GetCharacterPoints(monsterName);
        }

        score += pointsEarned;
        collectedMonsters.Add(monsterName);
        collectedPoints.Add(pointsEarned);

        Debug.Log($"✅ +{pointsEarned} from {monsterName} | Total Score: {score}");

        UpdateUI();

        if (BadgeManager.instance != null)
        {
            BadgeManager.instance.CheckBadge(score);
        }

        SaveScore();
    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    void SaveScore()
    {
        PlayerPrefs.SetInt("SCORE", score);
        PlayerPrefs.Save();
    }

    void LoadScore()
    {
        score = PlayerPrefs.GetInt("SCORE", 0);
    }
}

