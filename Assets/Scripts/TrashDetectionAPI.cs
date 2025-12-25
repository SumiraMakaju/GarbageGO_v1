using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class TrashDetectionAPI : MonoBehaviour
{
    public string apiUrl = "http://YOUR_SERVER/detect";

    public IEnumerator SendImage(Texture2D image)
    {
        byte[] jpg = image.EncodeToJPG();
        WWWForm form = new WWWForm();
        form.AddBinaryData("image", jpg, "frame.jpg", "image/jpeg");

        UnityWebRequest req = UnityWebRequest.Post(apiUrl, form);
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Detection result: " + req.downloadHandler.text);
            // JSON → bounding boxes
        }
        else
        {
            Debug.LogError("API error");
        }
    }
}
