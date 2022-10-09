using Assets.Scripts.Controller.InGame;
using System;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Data
{
    public class SelectionModel
    {
        public List<GameObject> SelectionObjects { get; private set; }
        public Type SelectionType { get; private set; }
        public bool HaveObjectSelected { get { return SelectionObjects != null && SelectionObjects.Count > 0; } }
        public Vector3? MultiselectOrigin;

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

        public void StartMultiselect(Vector3 startPoint, Type selectionType)
        {
            MultiselectOrigin = startPoint;
            SetAsNull();
            ChangeSelection(null, selectionType);
        }

        public void UpdateMultiselect(Vector3 multiselectEnd, Transform parentHolder, int thisPcPlayer)
        {
            if (MultiselectOrigin.HasValue)
            {
                Bounds bounds = new Bounds();

                if (MultiselectOrigin.Value.x > multiselectEnd.x)
                {
                    bounds.max = MultiselectOrigin.Value;
                    bounds.min = multiselectEnd;
                }
                else
                {
                    bounds.max = multiselectEnd;
                    bounds.min = MultiselectOrigin.Value;
                }

                foreach (Transform troopTransform in parentHolder.transform)
                {
                    if (bounds.Contains(troopTransform.position))
                    {
                        IObjectSelectable objectSelectable = troopTransform.GetComponent<IObjectSelectable>();
                        SetTroopSelected(objectSelectable, true, SelectionType, thisPcPlayer);
                    }
                }

                //Debug.Log($"Start {MultiselectOrigin} and end {multiselectEnd}");
            }
        }

        private void SetTroopSelected(IObjectSelectable newSelection, bool isMultiselect, Type type, int thisPcPlayer)
        {
            if (newSelection.IsSelectable(thisPcPlayer))
            {
                if (isMultiselect)
                {
                    AppendSelection(newSelection.GetGameObject());
                }
                else
                {
                    ChangeSelection(newSelection.GetGameObject(), type);
                }
            }
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
