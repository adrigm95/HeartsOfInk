using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Data.MapModels
{
    /// <summary>
    /// Forma parte del modelo MapModel, que define la estructura de los .json con la información de los mapas.
    /// </summary>
    [Serializable]
    public class MapFactionModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int BonusCode { get; set; }
        public int DefaultOwner { get; set; }
        public List<FactionDescriptionModel> Descriptions { get; set; }
    }
}
