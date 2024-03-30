using Assets.Scripts.Data.Literals;
using System;
using System.Collections.Generic;

public class LanguageManager
{
    private const string DefaultLanguage = "ES-es";
    private static string _language;

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
            literal = literals.Find(lit => lit.Language == DefaultLanguage);
        }

        return literal.Value;
    }

    private static string GetLanguage()
    {
        if (string.IsNullOrWhiteSpace(_language))
        {
            // TODO: obtener el idioma de las preferencias de usuario.
            _language = "ES-es";
        }

        return _language;
    }

    private static bool ValidateLanguage(string language)
    {
        switch (language)
        {
            case LanguageConstants.English:
            case LanguageConstants.Spanish_Spain:
            case LanguageConstants.Portuguese_Brazil:
            case LanguageConstants.Valencian:
                return true;
            default:
                return false;
        }
    }
}
