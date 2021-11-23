using System;
using UnityEngine;

namespace Assets.Scripts.Data
{
    public class SelectionModel
    {
        public GameObject SelectionObject { get; private set; }
        public Type SelectionType { get; private set; }
        public bool HaveObjectSelected { get { return SelectionObject != null; } }

        public void ChangeSelection(GameObject newSelection, Type newType)
        {
            SelectionObject = newSelection;
            SelectionType = newType;
        }

        public void SetAsNull()
        {
            SelectionObject = null;
            SelectionType = null;
        }
    }
}
