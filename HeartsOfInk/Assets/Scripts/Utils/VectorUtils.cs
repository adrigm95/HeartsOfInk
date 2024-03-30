using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    public static class VectorUtils
    {
        public static Vector3 StringToVector3(string position)
        {
            Vector3 vectorPosition;
            string[] splittedPosition;

            splittedPosition = position.Split(',');
            vectorPosition = new Vector3(Convert.ToSingle(splittedPosition[0]), Convert.ToSingle(splittedPosition[1]));

            return vectorPosition;
        }

        public static Vector3 FloatVectorToVector3(float[] position)
        {
            return new Vector3(position[0], position[1]);
        }
    }
}
