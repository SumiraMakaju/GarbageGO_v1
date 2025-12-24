using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public int score = 0;
    public TextMeshProUGUI scoreText;

    [Header("Badge UI")]
    public GameObject badgePopupPanel;
    public Image medalImage;
    public TextMeshProUGUI badgeText;

    [Header("Medal Sprites")]
    public Sprite bronzeMedal;
    public Sprite silverMedal;
    public Sprite goldMedal;

    bool bronzeUnlocked, silverUnlocked, goldUnlocked;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void AddScore(int value)
    {
        score += value;
        scoreText.text = "Score: " + score;
        CheckBadges();
    }

    void CheckBadges()
    {
        if (score >= 50 && !bronzeUnlocked)
        {
            bronzeUnlocked = true;
            StartCoroutine(ShowBadge(bronzeMedal, "🥉 Cleaner Unlocked!"));
        }
        else if (score >= 100 && !silverUnlocked)
        {
            silverUnlocked = true;
            StartCoroutine(ShowBadge(silverMedal, "🥈 Eco Hero Unlocked!"));
        }
        else if (score >= 200 && !goldUnlocked)
        {
            goldUnlocked = true;
            StartCoroutine(ShowBadge(goldMedal, "🥇 City Guardian Unlocked!"));
        }
    }

    IEnumerator ShowBadge(Sprite medal, string text)
    {
        medalImage.sprite = medal;
        badgeText.text = text;

        badgePopupPanel.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        badgePopupPanel.SetActive(false);
    }
}
