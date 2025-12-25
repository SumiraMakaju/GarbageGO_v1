using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class BadgeManager : MonoBehaviour
{
    [System.Serializable]
    public class Badge
    {
        public string badgeName;
        public int pointsRequired;
        public string badgeEmoji;
        public string description;
    }

    [System.Serializable]
    public class CharacterPoints
    {
        public string characterName;
        public int pointValue = 1;
    }

    public static BadgeManager instance;

    public GameObject badgePopup;
    public TextMeshProUGUI badgeText;
    public TextMeshProUGUI descriptionText;
    
    public Badge[] badges = new Badge[]
    {
        new Badge { badgeName = "Bronze Collector", pointsRequired = 10, badgeEmoji = "🥉", description = "Collected 10 items" },
        new Badge { badgeName = "Silver Collector", pointsRequired = 25, badgeEmoji = "🥈", description = "Collected 25 items" },
        new Badge { badgeName = "Gold Collector", pointsRequired = 50, badgeEmoji = "🥇", description = "Collected 50 items" },
        new Badge { badgeName = "Platinum Collector", pointsRequired = 100, badgeEmoji = "💎", description = "Collected 100 items" },
        new Badge { badgeName = "Diamond Master", pointsRequired = 200, badgeEmoji = "👑", description = "Collected 200 items" },
    };

    public CharacterPoints[] characterPointValues = new CharacterPoints[]
    {
        new CharacterPoints { characterName = "DragonNightmare_Blue", pointValue = 1 },
        new CharacterPoints { characterName = "DragonNightmare_Green", pointValue = 1 },
        new CharacterPoints { characterName = "DragonSoulEater_Blue", pointValue = 2 },
        new CharacterPoints { characterName = "DragonSoulEater_Red", pointValue = 2 },
        new CharacterPoints { characterName = "DragonTerrorBringer_Purple", pointValue = 3 },
        new CharacterPoints { characterName = "DragonTerrorBringer_Blue", pointValue = 3 },
        new CharacterPoints { characterName = "DragonUsurper_Green", pointValue = 5 },
        new CharacterPoints { characterName = "DragonUsurper_Purple", pointValue = 5 },
    };

    private HashSet<int> unlockedBadges = new HashSet<int>();
    private float badgeDisplayDuration = 3f;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        if (badgePopup != null)
            badgePopup.SetActive(false);
    }

    /// <summary>
    /// Get point value for a specific character
    /// Default: 1 point if character not found
    /// </summary>
    public int GetCharacterPoints(string characterName)
    {
        foreach (var charPoints in characterPointValues)
        {
            if (charPoints.characterName.Contains(characterName))
                return charPoints.pointValue;
        }
        Debug.LogWarning($"BadgeManager: Character '{characterName}' not found in point values, using default 1 point");
        return 1;
    }

    /// <summary>
    /// Check if new badges should be unlocked
    /// Call this whenever score changes
    /// </summary>
    public void CheckBadge(int totalScore)
    {
        for (int i = 0; i < badges.Length; i++)
        {
            if (totalScore >= badges[i].pointsRequired && !unlockedBadges.Contains(i))
            {
                UnlockBadge(i);
            }
        }
    }

    /// <summary>
    /// Unlock and display a badge
    /// </summary>
    void UnlockBadge(int badgeIndex)
    {
        if (badgeIndex >= 0 && badgeIndex < badges.Length)
        {
            unlockedBadges.Add(badgeIndex);
            Badge badge = badges[badgeIndex];
            ShowBadge(badge);
            Debug.Log($"[Badge] UNLOCKED: {badge.badgeName} at {badge.pointsRequired} points!");
        }
    }

    /// <summary>
    /// Display badge popup with animation
    /// </summary>
    void ShowBadge(Badge badge)
    {
        if (badgePopup == null || badgeText == null)
            return;

        string displayText = $"{badge.badgeEmoji}\n{badge.badgeName}";
        badgeText.text = displayText;
        
        if (descriptionText != null)
            descriptionText.text = badge.description;

        badgePopup.SetActive(true);
        
        // Cancel any existing hide invoke
        CancelInvoke("HideBadge");
        
        // Schedule hide
        Invoke("HideBadge", badgeDisplayDuration);
    }

    void HideBadge()
    {
        if (badgePopup != null)
            badgePopup.SetActive(false);
    }

    /// <summary>
    /// Get all unlocked badges
    /// </summary>
    public Badge[] GetUnlockedBadges()
    {
        List<Badge> unlocked = new List<Badge>();
        foreach (int index in unlockedBadges)
        {
            if (index >= 0 && index < badges.Length)
                unlocked.Add(badges[index]);
        }
        return unlocked.ToArray();
    }

    /// <summary>
    /// Get next badge to unlock
    /// </summary>
    public Badge GetNextBadge(int currentScore)
    {
        for (int i = 0; i < badges.Length; i++)
        {
            if (currentScore < badges[i].pointsRequired && !unlockedBadges.Contains(i))
                return badges[i];
        }
        return null;
    }

    /// <summary>
    /// Get progress to next badge
    /// </summary>
    public float GetProgressToNextBadge(int currentScore)
    {
        Badge nextBadge = GetNextBadge(currentScore);
        if (nextBadge == null)
            return 1f; // All badges unlocked

        Badge prevBadge = null;
        int prevPoints = 0;

        for (int i = 0; i < badges.Length; i++)
        {
            if (badges[i].pointsRequired > currentScore)
                break;
            prevBadge = badges[i];
            prevPoints = badges[i].pointsRequired;
        }

        if (prevBadge == null)
            prevPoints = 0;

        int range = nextBadge.pointsRequired - prevPoints;
        int progress = currentScore - prevPoints;
        
        return Mathf.Clamp01((float)progress / range);
    }
}
