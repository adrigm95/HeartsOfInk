using Assets.Scripts.Data;
using UnityEngine;
using UnityEngine.UI;

public class EndgameLineController : MonoBehaviour
{
    private Rawgen.Literals.LiteralsFactory.Language language;
    public string factionId;

    void Start()
    {
        language = Rawgen.Literals.LiteralsFactory.Language.es_ES;
        SetFactionName();
    }

    private void SetFactionName()
    {
        Text txtFaction = transform.Find("txtFaction").GetComponent<Text>();
        txtFaction.text = factionId;
    }
}
