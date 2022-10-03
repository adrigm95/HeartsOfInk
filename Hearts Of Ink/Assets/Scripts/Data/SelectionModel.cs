using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Data
{
    public class SelectionModel
    {
        public List<GameObject> SelectionObjects { get; private set; }
        public Type SelectionType { get; private set; }
        public bool HaveObjectSelected { get { return SelectionObjects != null && SelectionObjects.Count > 0; } }

        public void ChangeSelection(GameObject newSelection, Type newType)
        {
            OverwriteSelection(newSelection);
            SelectionType = newType;
        }

        public void AppendSelection(GameObject newSelection)
        {
            if (newSelection != null && !SelectionObjects.Contains(newSelection))
            {
                SelectionObjects.Add(newSelection);
            }
        }

        public void SetAsNull()
        {
            if (SelectionObjects != null)
            {
                SelectionObjects.Clear();
            }
            
            SelectionType = null;
        }

        private void OverwriteSelection(GameObject newSelection)
        {
            if (SelectionObjects == null)
            {
                SelectionObjects = new List<GameObject>();
            }
            else
            {
                SelectionObjects.Clear();
            }

            AppendSelection(newSelection);
        }
    }
}
