namespace Assets.Scripts.Utils
{
    public static class FileUtils
    {
        public static string SanitizeFilename(string filename)
        {
            string invalidCharacters = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegString = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidCharacters);

            return System.Text.RegularExpressions.Regex.Replace(filename, invalidRegString, "-");
        }
    }
}
