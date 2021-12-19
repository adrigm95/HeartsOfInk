using UnityEngine;
using UnityEngine.UI;

public class InfoPanelController : MonoBehaviour
{
    private TutorialController tutorialController;
    private bool closeOnAccept;
    public Text txtTitle;
    public Text txtContent;
    public Button cancel;

    public void DisplayMessage(TutorialController tutorialController, string title, string content, bool closeOnAccept)
    {
        this.tutorialController = tutorialController;
        this.closeOnAccept = closeOnAccept;
        DisplayMessage(title, content);
    }

    public void DisplayDecision(TutorialController tutorialController, string title, string content, bool closeOnAccept)
    {
        this.tutorialController = tutorialController;
        this.closeOnAccept = closeOnAccept;
        cancel.gameObject.SetActive(true);
        DisplayMessage(title, content);
    }

    public void DisplayMessage(string title, string content)
    {
        txtTitle.text = title;
        txtContent.text = content;
        this.gameObject.SetActive(true);
        Debug.Log("Displaying message for title: " + title);
    }

    public void Accept()
    {
        cancel.gameObject.SetActive(false);

        if (closeOnAccept)
        {
            this.gameObject.SetActive(false);
        }
        else if (tutorialController != null)
        {
            if (!tutorialController.NextMessage())
            {
                this.gameObject.SetActive(false);
            }
        }
    }

    public void Cancel()
    {
        cancel.gameObject.SetActive(false);
        this.gameObject.SetActive(false);

        if (tutorialController != null)
        {
            tutorialController.DiscardTutorial();
        }
    }
}
