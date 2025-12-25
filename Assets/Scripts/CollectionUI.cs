using UnityEngine;
using TMPro;

public class CollectionUI : MonoBehaviour
{
    public GameObject collectionPanel;
    public Transform contentParent;
    public GameObject textPrefab;

    public void OpenCollection()
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        foreach (string monster in ScoreManager.instance.collectedMonsters)
        {
            GameObject t = Instantiate(textPrefab, contentParent);
            t.GetComponent<TextMeshProUGUI>().text = "👾 " + monster;
        }

        collectionPanel.SetActive(true);
    }

    public void CloseCollection()
    {
        collectionPanel.SetActive(false);
    }
}
