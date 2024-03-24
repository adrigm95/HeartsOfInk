using Newtonsoft.Json;
using System;
using UnityEngine;

namespace Assets.Scripts.Data.MultiplayerStateModels
{
    public class TroopStateModel
    {
        /// <summary>
        /// Posición de la tropa
        /// </summary>
        public string position;

        /// <summary>
        /// Tamaño de la tropa
        /// </summary>
        public int size;

        public Vector3 GetPositionAsVector3()
        {
            if (position != null)
            {
                string[] tmp = position.Split(';');
                return new Vector3(Convert.ToSingle(tmp[0]), Convert.ToSingle(tmp[1]), Convert.ToSingle(tmp[2]));
            }
            else
            {
                return Vector3.zero;
            }
        }

        public void SetPosition(Vector3 newValue)
        {
            position = newValue.x + ";" + newValue.y + ";" + newValue.z;
        }
    }
}
