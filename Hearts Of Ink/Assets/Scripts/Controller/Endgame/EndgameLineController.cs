using Assets.Scripts.Data;
using UnityEngine;
using UnityEngine.UI;

public class EndgameLineController : MonoBehaviour
{
    private Rawgen.Literals.LiteralsFactory.Language language;
    public Faction.Id factionId;

    void Start()
    {
        language = Rawgen.Literals.LiteralsFactory.Language.es_ES;
        SetFactionName();
    }

    private void SetFactionName()
    {
        Text txtFaction = transform.Find("txtFaction").GetComponent<Text>();

        switch (factionId)
        {
            case Faction.Id.REBELS:
                txtFaction.text = FactionNames.GetInstance(language).RebelsName;
                break;
            case Faction.Id.GOVERNMENT:
                txtFaction.text = FactionNames.GetInstance(language).GovernmentName;
                break;
            case Faction.Id.VUKIS:
                txtFaction.text = FactionNames.GetInstance(language).VukisName;
                break;
            case Faction.Id.NOMADS:
                txtFaction.text = FactionNames.GetInstance(language).NomadsName;
                break;
        }
    }
}
