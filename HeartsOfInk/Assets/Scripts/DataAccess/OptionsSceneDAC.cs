using HeartsOfInk.SharedLogic;
using UnityEngine;

namespace Assets.Scripts.DataAccess
{
    /// <summary>
    /// Carga y guardado del fichero de opciones.
    /// </summary>
    public class OptionsSceneDAC
    {
        public static OptionsModel LoadOptionsPreferences()
        {
            string optionsPreferencesPath = GetOptionsFilePath();

            return JsonCustomUtils<OptionsModel>.ReadObjectFromFile(optionsPreferencesPath);
        }

        public static void SaveOptionsPreferences(OptionsModel optionsPreferences)
        {
            string optionsPreferencesPath = GetOptionsFilePath();

            JsonCustomUtils<OptionsModel>.SaveObjectIntoFile(optionsPreferences, optionsPreferencesPath);
        }

        /// <summary>
        /// Obtiene la dirección donde está guardado el fichero de opciones.
        /// </summary>
        /// <returns></returns>
        private static string GetOptionsFilePath()
        {
            // Guardamos y cargamos el fichero de opciones en persistentDataPath por diferentes razones:
            // - Si lo guardamos en StreamingAssets, podríamos modificar las opciones 
            // por defecto sin querer y publicar el juego con unas opciones por defecto no deseadas.
            // - Al guardarlo en una carpeta ajena al proyecto, si alguien se instala una versión nueva
            // sigue manteniendo las opciones que ya tuviera guardadas.
            return Application.persistentDataPath + OptionsModel.PreferencesInfoFile;
        }
    }
}