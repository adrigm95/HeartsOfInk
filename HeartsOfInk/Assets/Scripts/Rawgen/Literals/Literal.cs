using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawgen.Literals
{
    public class Literal
    {
        private string en;
        private string es;
        private string es_ES;
        private string ca;
        private string ca_va;

        public void SetValue(string value, LiteralsFactory.Language languageCode)
        {
            switch (languageCode)
            {

                case LiteralsFactory.Language.es:
                    es = value;
                    break;
                case LiteralsFactory.Language.es_ES:
                    es_ES = value;
                    break;
                case LiteralsFactory.Language.en:
                    en = value;
                    break;
                case LiteralsFactory.Language.ca:
                    ca = value;
                    break;
                case LiteralsFactory.Language.ca_va:
                    ca_va = value;
                    break;
            }
        }

        public string GetValue(LiteralsFactory.Language languageCode)
        {
            switch (languageCode)
            {

                case LiteralsFactory.Language.es:
                    return es;
                case LiteralsFactory.Language.es_ES:
                    return es_ES;
                case LiteralsFactory.Language.en:
                    return en;
                case LiteralsFactory.Language.ca:
                    return ca;
                case LiteralsFactory.Language.ca_va:
                    return ca_va;
                default:
                    return en;
            }
        }
    }
}
