using HeartsOfInk.SharedLogic;
using System.IO;

namespace Assets.Scripts.DataAccess
{
    /// <summary>
    /// Carga y guardado del fichero de opciones.
    /// </summary>
    public class FileDAC<T>
    {
        public static T LoadFile(string path)
        {
            if (File.Exists(path))
            {
                return JsonCustomUtils<T>.ReadObjectFromFile(path);

            } 
            else
            {
                return default;
            }
        }

        /// <summary>
        /// Guarda el contenido serializado en json en un fichero. Si ya existe lo sobrescribe.
        /// </summary>
        /// <param name="fileContent"> Contenido del fichero.</param>
        /// <param name="path"> Ruta.</param>
        public static void SaveFile(T fileContent, string path)
        {
            JsonCustomUtils<T>.SaveObjectIntoFile(fileContent, path);
        }
    }
}