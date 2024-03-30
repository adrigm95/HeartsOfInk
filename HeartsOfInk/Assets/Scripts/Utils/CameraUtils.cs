using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    public static class CameraUtils
    {
        public static Vector3 ScreenToWorldPoint(Vector3 position)
        {
            Vector3 cameraPosition = Camera.main.transform.position;

            position = Camera.main.ScreenToViewportPoint(position);
            position = Camera.main.ViewportToWorldPoint(position);
            position *= -1f;
            position.x += cameraPosition.x * 2;
            position.y += cameraPosition.y * 2;

            return position;
        }
    }
}
