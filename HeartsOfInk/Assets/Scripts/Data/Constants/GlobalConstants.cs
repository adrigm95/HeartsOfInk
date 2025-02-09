using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Data
{
    public static class GlobalConstants
    {
        public static readonly string RootPath = Application.persistentDataPath;
        public const string EmptyTargetName = "Unit target position";
        public const string FactionLineStart = "factionLine";
        public const int GuerrillaLimit = 100;
        public const int DefaultCompanySize = 80;
    }
}
