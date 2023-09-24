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
            return JsonConvert.DeserializeObject<Vector3>(position);
        }

        public void SetPosition(Vector3 newValue)
        {
            position = JsonConvert.SerializeObject(newValue);
        }
    }
}
