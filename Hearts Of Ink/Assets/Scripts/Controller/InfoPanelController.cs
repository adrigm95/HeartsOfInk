using UnityEngine;
using UnityEngine.UI;

public class InfoPanelController : MonoBehaviour
{
    public Text txtTitle;
    public Text txtContent;

    public void DisplayMessage(string title, string content)
    {
        txtTitle.text = title;
        txtContent.text = content;
        this.gameObject.SetActive(true);
    }
}
