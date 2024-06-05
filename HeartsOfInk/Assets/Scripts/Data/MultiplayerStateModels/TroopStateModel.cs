using System;
using UnityEngine;

namespace Assets.Scripts.Data.MultiplayerStateModels
{
    public class TroopStateModel
    {
        /// <summary>
        /// Posición de la tropa
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// Tamaño de la tropa
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Identificador del dueño actual de la tropa.
        /// </summary>
        public byte MapPlayerSlotId { get; set; }

        public Vector3 GetPositionAsVector3()
        {
            if (Position != null)
            {
                string[] tmp = Position.Split(';');
                return new Vector3(Convert.ToSingle(tmp[0]), Convert.ToSingle(tmp[1]), Convert.ToSingle(tmp[2]));
            }
            else
            {
                return Vector3.zero;
            }
        }

        public void SetPosition(Vector3 newValue)
        {
            Position = newValue.x + ";" + newValue.y + ";" + newValue.z;
        }

        public override string ToString()
        {
            return "Position: " + Position + " Size: " + Size + " Owner: " + MapPlayerSlotId;
        }
    }
}
