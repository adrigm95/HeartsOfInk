using System.Collections.Generic;

namespace Assets.Scripts.Data.Literals
{
    public class MainMenuLiterals
    {
        // Panel izquierdo
        public List<LiteralModel> FastGame { get; set; }
        public List<LiteralModel> Levels { get; set; }
        public List<LiteralModel> Multiplayer { get; set; }
        public List<LiteralModel> Options { get; set; }
        public List<LiteralModel> Editor { get; set; }
        public List<LiteralModel> Credits { get; set; }
        public List<LiteralModel> ExitGame { get; set; }

        // Panel derecho
        public List<LiteralModel> TxtChooseMap { get; set; }
        public List<LiteralModel> TxtYourFactionTitle { get; set; }
        public List<LiteralModel> TxtFactionBonusTitle { get; set; }

        public string FastGameLiteral
        {
            get
            {
                return LanguageManager.GetLiteral(FastGame);
            }
        }

        public string LevelsLiteral
        {
            get
            {
                return LanguageManager.GetLiteral(Levels);
            }
        }

        public string MultiplayerLiteral
        {
            get
            {
                return LanguageManager.GetLiteral(Multiplayer);
            }
        }

        public string OptionsLiteral
        {
            get
            {
                return LanguageManager.GetLiteral(Options);
            }
        }

        public string EditorLiteral
        {
            get
            {
                return LanguageManager.GetLiteral(Editor);
            }
        }

        public string CreditsLiteral
        {
            get
            {
                return LanguageManager.GetLiteral(Credits);
            }
        }

        public string ExitGameLiteral
        {
            get
            {
                return LanguageManager.GetLiteral(ExitGame);
            }
        }

        public string TxtChooseMapLiteral
        {
            get
            {
                return LanguageManager.GetLiteral(TxtChooseMap);
            }
        }

        public string TxtYourFactionTitleLiteral
        {
            get
            {
                return LanguageManager.GetLiteral(TxtYourFactionTitle);
            }
        }

        public string TxtFactionBonusTitleLiteral
        {
            get
            {
                return LanguageManager.GetLiteral(TxtFactionBonusTitle);
            }
        }
    }
}
