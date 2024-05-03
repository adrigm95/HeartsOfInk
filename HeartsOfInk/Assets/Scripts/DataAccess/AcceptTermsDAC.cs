using HeartsOfInk.SharedLogic;
using System;
using UnityEngine;

namespace Assets.Scripts.DataAccess
{
    public class AcceptTermsDAC
    {
        private const string FileName = "/acceptTerms.txt";
        public static void SaveAcceptTerms()
        {
            JsonCustomUtils<string>.SaveObjectIntoFile(DateTime.Now.Ticks.ToString(), GetFile());
            Debug.Log(GetFile());
        }
        private static string GetFile()
        {
            return Application.persistentDataPath + FileName;
        }
    }
}
