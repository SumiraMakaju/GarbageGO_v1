using UnityEngine;
using TMPro;

public class BadgeManager : MonoBehaviour
{
    public static BadgeManager instance;

    public GameObject badgePopup;
    public TextMeshProUGUI badgeText;

    void Awake()
    {
        instance = this;
    }

    public void CheckBadge(int score)
    {
        if (score == 1)
            ShowBadge("🥉 Bronze Hunter Unlocked!");
        else if (score == 5)
            ShowBadge("🥈 Silver Hunter Unlocked!");
        else if (score == 10)
            ShowBadge("🥇 Gold Hunter Unlocked!");
    }

    void ShowBadge(string text)
    {
        badgeText.text = text;
        badgePopup.SetActive(true);
        Invoke("HideBadge", 2f);
    }

    void HideBadge()
    {
        badgePopup.SetActive(false);
    }
}
