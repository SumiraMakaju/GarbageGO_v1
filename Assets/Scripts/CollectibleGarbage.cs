using System.Collections;
using TMPro;
using UnityEngine;

public class CollectibleGarbage : MonoBehaviour
{
    public int points = 1;
    public TextMeshProUGUI plusOneText;

    void OnMouseDown()
    {
        ScoreManager.instance.AddScore(points);
        StartCoroutine(ShowPlusOne());
    }

    IEnumerator ShowPlusOne()
    {
        plusOneText.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
