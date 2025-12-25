using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public int score = 0;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI badgeProgressText; // Optional: shows next badge progress

    public List<string> collectedMonsters = new List<string>();
    public List<int> collectedPoints = new List<int>();

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        
        // Load score from PlayerPrefs
        LoadScore();
    }
    
    void OnDestroy()
    {
        // Save score when the app closes
        SaveScore();
    }

    /// <summary>
    /// Add monster to collection with dynamic points based on character type
    /// </summary>
    public void AddMonster(string monsterName)
    {
        // Get point value from BadgeManager (character-specific)
        int pointsEarned = 1; // Default
        if (BadgeManager.instance != null)
        {
            pointsEarned = BadgeManager.instance.GetCharacterPoints(monsterName);
        }

        score += pointsEarned;
        collectedMonsters.Add(monsterName);
        collectedPoints.Add(pointsEarned);
        
        UpdateUI();

        // Check for badge unlock
        if (BadgeManager.instance != null)
        {
            BadgeManager.instance.CheckBadge(score);
            UpdateBadgeProgress();
        }

        Debug.Log($"[ScoreManager] +{pointsEarned} points for {monsterName}! Total: {score}");
        
        // Save score to PlayerPrefs
        SaveScore();
    }
    
    void SaveScore()
    {
        PlayerPrefs.SetInt("PlayerScore", score);
        PlayerPrefs.SetInt("MonsterCount", collectedMonsters.Count);
        
        // Save individual monsters and points
        for (int i = 0; i < collectedMonsters.Count; i++)
        {
            PlayerPrefs.SetString($"Monster_{i}", collectedMonsters[i]);
            PlayerPrefs.SetInt($"Points_{i}", collectedPoints[i]);
        }
        
        PlayerPrefs.Save();
        Debug.Log($"[ScoreManager] Score saved: {score}");
    }
    
    void LoadScore()
    {
        score = PlayerPrefs.GetInt("PlayerScore", 0);
        int monsterCount = PlayerPrefs.GetInt("MonsterCount", 0);
        
        collectedMonsters.Clear();
        collectedPoints.Clear();
        
        // Load individual monsters and points
        for (int i = 0; i < monsterCount; i++)
        {
            string monsterName = PlayerPrefs.GetString($"Monster_{i}", "");
            int points = PlayerPrefs.GetInt($"Points_{i}", 0);
            
            if (!string.IsNullOrEmpty(monsterName))
            {
                collectedMonsters.Add(monsterName);
                collectedPoints.Add(points);
            }
        }
        
        UpdateUI();
        Debug.Log($"[ScoreManager] Score loaded: {score} (Monsters: {monsterCount})");
    }

    /// <summary>
    /// Update all UI elements
    /// </summary>
    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = score.ToString();
    }

    /// <summary>
    /// Update badge progress display
    /// </summary>
    void UpdateBadgeProgress()
    {
        if (badgeProgressText == null || BadgeManager.instance == null)
            return;

        BadgeManager.Badge nextBadge = BadgeManager.instance.GetNextBadge(score);
        if (nextBadge != null)
        {
            float progress = BadgeManager.instance.GetProgressToNextBadge(score);
            badgeProgressText.text = $"Next: {nextBadge.badgeName} ({Mathf.RoundToInt(progress * 100)}%)";
        }
        else
        {
            badgeProgressText.text = "All badges unlocked!";
        }
    }

    /// <summary>
    /// Get total monsters collected
    /// </summary>
    public int GetMonsterCount()
    {
        return collectedMonsters.Count;
    }

    /// <summary>
    /// Get breakdown of points by character
    /// </summary>
    public Dictionary<string, int> GetPointsBreakdown()
    {
        Dictionary<string, int> breakdown = new Dictionary<string, int>();
        for (int i = 0; i < collectedMonsters.Count; i++)
        {
            string monsterName = collectedMonsters[i];
            int points = collectedPoints[i];

            if (breakdown.ContainsKey(monsterName))
                breakdown[monsterName] += points;
            else
                breakdown[monsterName] = points;
        }
        return breakdown;
    }
}
