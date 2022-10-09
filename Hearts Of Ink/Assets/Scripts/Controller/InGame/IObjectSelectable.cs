using Assets.Scripts.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Controller.InGame
{
    public interface IObjectSelectable
    {
        bool IsSelectable(int owner);
        GameObject GetGameObject();
    }
}
