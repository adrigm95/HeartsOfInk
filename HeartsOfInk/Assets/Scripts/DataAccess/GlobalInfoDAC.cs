using Assets.Scripts.Data.GlobalInfo;
using HeartsOfInk.SharedLogic;
using UnityEngine;

namespace Assets.Scripts.DataAccess
{
    internal class GlobalInfoDAC
    {
        public const string GlobalInfoFile = "/_GlobalInfo.json";

        public static GlobalInfo LoadGlobalMapInfo()
        {
            string globalInfoPath = Application.persistentDataPath + GlobalInfoFile;

            return JsonCustomUtils<GlobalInfo>.ReadObjectFromFile(globalInfoPath);
        }
    }
}
