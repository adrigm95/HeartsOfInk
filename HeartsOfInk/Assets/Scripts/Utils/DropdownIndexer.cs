using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Utils
{
    public class DropdownIndexer
    {
        private List<int> itemIds;
        private Dictionary<int, string> itemTexts;

        public DropdownIndexer()
        {
            itemIds = new List<int>();
            itemTexts = new Dictionary<int, string>();
        }

        public void AddRegister(int itemId, string itemText)
        {
            itemTexts.Add(itemId, itemText);
            itemIds.Add(itemId);
        }

        public List<Dropdown.OptionData> GetOptions()
        {
            List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();

            for (int valueId = 0; valueId < itemIds.Count; valueId++)
            {
                int itemId = itemIds[valueId];
                options.Add(new Dropdown.OptionData(itemTexts[itemId]));
            }

            return options;
        }

        public int GetValue(int itemId)
        {
            return itemIds.FindIndex(item => item == itemId);
        }
    }
}
