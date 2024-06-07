using Assets.Scripts.Data.Literals;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class LanguageManager
{
    private static string _language;

    public static string DefaultLanguage
    {
        get
        {
            return LanguageConstants.Spanish_Spain;
        }
    }

    public static string Language { 
        get
        {
            return GetLanguage();
        }

        set
        {
            if (ValidateLanguage(value))
            {
                _language = value;
            }
            else
            {
                throw new ArgumentException($"Received language code is not valid: {value}");
            }
        }
    }

    public static string GetLiteral(List<LiteralModel> literals)
    {
        LiteralModel literal = literals.Find(lit => lit.Language == Language);

        if (literal == null)
        {
            // Si no encuentra literal del idioma requerido, busca del idioma por defecto.
            literal = literals.Find(lit => lit.Language == DefaultLanguage);

            if (literal == null)
            {
                // Si el idioma por defecto tampoco tiene literal, se devuelve el primero que haya.
                literal = literals.First();
            }
        }

        return literal.Value;
    }

    private static string GetLanguage()
    {
        if (string.IsNullOrWhiteSpace(_language))
        {
            string optionsLanguage = OptionsManager.Instance.OptionsModel.Language;
            if (ValidateLanguage(optionsLanguage))
            {
                _language = optionsLanguage;
            }
            else 
            {
                _language = "ES-es";
            }
        }

        return _language;
    }

    private static bool ValidateLanguage(string language)
    {
        switch (language)
        {
            case LanguageConstants.English:
            case LanguageConstants.Spanish_Spain:
            case LanguageConstants.Valencian:
            case LanguageConstants.Catalonian:
                return true;
            case LanguageConstants.Portuguese_Brazil:
            default:
                return false;
        }
    }
}
