using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rawgen.Unity.Editor.Utils
{
    public static class FoldersUtils
    {
        public static string EraseStreamingAssetsPath(string fullpath)
        {
            return fullpath.Replace(Application.streamingAssetsPath, string.Empty);
        }

        public static string AddStreamingAssetsPath(string relativePath)
        {
            return Application.streamingAssetsPath + "/" + relativePath;
        }
    }
}
