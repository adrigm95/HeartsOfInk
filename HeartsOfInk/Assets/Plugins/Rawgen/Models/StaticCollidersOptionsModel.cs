using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rawgen.Unity.Editor.Models
{
    public class StaticCollidersOptionsModel
    {
        public string colliderPath { get; set; }
        public Vector2 offset { get; set; }
        public float scale { get; set; }
        public bool loadAutomatically { get; set; }
        public bool overwriteOptions { get; set; }
    }
}
