using Assets.Scripts.Data.Literals;
using Assets.Scripts.DataAccess;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuLiteralsController : MonoBehaviour
{
    private MainMenuLiterals literals;

    // Panel izquierdo
    public Text txtBtnFastGame;
    public Text txtBtnLevels;
    public Text txtBtnMultiplayer;
    public Text txtBtnOptions;
    public Text txtBtnEditor;
    public Text txtBtnCredits;
    public Text txtBtnExitGame;

    // Panel derecho
    public Text txtChooseMap;
    public Text txtYourFactionTitle;
    public Text txtYourFactionBonusTitle;

    private void Start()
    {
        literals = LiteralsDAC<MainMenuLiterals>.LoadLiteralsFile("MainMenu.json");
        txtBtnFastGame.text = literals.FastGameLiteral;
        txtBtnLevels.text = literals.LevelsLiteral;
        txtBtnMultiplayer.text = literals.MultiplayerLiteral;
        txtBtnOptions.text = literals.OptionsLiteral;
        txtBtnEditor.text = literals.EditorLiteral;
        txtBtnCredits.text = literals.CreditsLiteral;
        txtBtnExitGame.text = literals.ExitGameLiteral;

        txtChooseMap.text = literals.TxtChooseMapLiteral;
        txtYourFactionTitle.text = literals.TxtYourFactionTitleLiteral;
        txtYourFactionBonusTitle.text = literals.TxtFactionBonusTitleLiteral;

        Debug.Log("Main menu literals loaded for language " + OptionsManager.Instance.OptionsModel.Language);
    }
}
