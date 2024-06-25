using Assets.Scripts.Data.Literals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Data.GlobalInfo
{
    public class GlobalInfoFaction
    {
        public int Id { get; set; }
        public int BonusId { get; set; }

        /// <summary>
        /// Es el nombre de la facción, por ejemplo: "Rebeldes"
        /// </summary>
        public List<LiteralModel> Names { get; set; }
        public List<LiteralModel> Descriptions { get; set; }

        /// <summary>
        /// Nombre del general si la facción la maneja la IA.
        /// </summary>
        public List<string> IANames { get; set; }

        public string NameLiteral 
        { 
            get
            {
                return LanguageManager.GetLiteral(Names);
            } 
        }

        public string DescriptionLiteral
        {
            get
            {
                return LanguageManager.GetLiteral(Descriptions);
            }
        }
    }
}
