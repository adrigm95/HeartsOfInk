using HeartsOfInk.SharedLogic;
using System;
using UnityEngine;

namespace Assets.Scripts.DataAccess
{
    public class AcceptTermsDAC
    {
        private const string FileName = "/acceptTerms.txt";

        public static string LoadAcceptTerms()
        {
            try
            {
                return JsonCustomUtils<string>.ReadObjectFromFile(GetFile());
            }
            catch (Exception ex)
            {
                return null;
            }
        }

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
