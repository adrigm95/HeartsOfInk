using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawgen.Literals
{
    public class LiteralsFactory
    {
        public enum Language
        {
            es = 100, // Espanol
            es_ES = 101, // Espanol de España
            en = 200, // Ingles
            ca = 300, // Catalan
            ca_va = 301, // Valenciano
        }

        protected Language languageCode;

        public Language LanguageCode { get => languageCode; set => languageCode = value; }

        public LiteralsFactory(Language languageCode)
        {
            this.languageCode = languageCode;
        }
    }
}
