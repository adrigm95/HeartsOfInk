using Assets.Scripts.Data.GlobalInfo;
using HeartsOfInk.SharedLogic;
using UnityEngine;

namespace Assets.Scripts.DataAccess
{
    internal class LiteralsDAC<T>
    {
        public static T LoadLiteralsFile(string filename)
        {
            string filePath = Application.persistentDataPath + "/Literals/" + filename;

            return JsonCustomUtils<T>.ReadObjectFromFile(filePath);
        }
    }
}
