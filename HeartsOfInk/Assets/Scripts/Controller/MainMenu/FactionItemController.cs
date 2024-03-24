using UnityEngine;
using UnityEngine.UI;

public class FactionItemController : MonoBehaviour
{
    public ConfigGameController configGameController;
    public Text txtAlliance;

    public void OnAllianceChange()
    {
        Debug.Log("OnAllianceChange - Start");

        switch (txtAlliance.text)
        {
            case "":
                txtAlliance.text = "1";
                break;
            case "1":
                txtAlliance.text = "2";
                break;
            case "2":
                txtAlliance.text = "3";
                break;
            case "3":
                txtAlliance.text = "4";
                break;
            case "4":
                txtAlliance.text = "";
                break;
            default:
                Debug.LogWarning("Unexpected value for txtAlliance: " + txtAlliance.text);
                break;
        }

        if (configGameController != null)
        {
            configGameController.OnChangeConfigLine(txtAlliance.transform.parent.transform.parent);
        }
    }
}
