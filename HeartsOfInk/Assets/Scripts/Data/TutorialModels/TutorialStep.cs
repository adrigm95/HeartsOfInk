using Assets.Scripts.Data.Literals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Data.TutorialModels
{
    public class TutorialStep
    {
        public int Number { get; set; }
        public bool PauseGame { get; set; }
        public List<LiteralModel> Title { get; set; }
        public List<LiteralModel> Content { get; set; }
    }
}
