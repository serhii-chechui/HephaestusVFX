using System.IO;

namespace WTFGames.Hephaestus.VFX.Editor
{
    /// <summary>
    /// Converts enum export paths between the project-relative form stored in assets and absolute paths.
    /// </summary>
    internal static class VFXProjectPaths
    {
        /// <summary>
        /// Resolves a project-relative path against <paramref name="projectRoot"/>; absolute paths are returned as is.
        /// </summary>
        public static string ToAbsolute(string path, string projectRoot)
        {
            if (string.IsNullOrEmpty(path)) return string.Empty;

            return Path.IsPathRooted(path) ? path : Path.Combine(projectRoot, path);
        }

        /// <summary>
        /// Stores folders inside the project relative to its root, so the path works on every machine.
        /// </summary>
        public static string ToProjectRelative(string absolutePath, string projectRoot)
        {
            var root = projectRoot.Replace('\\', '/').TrimEnd('/') + "/";
            var path = absolutePath.Replace('\\', '/');

            return path.StartsWith(root) ? path.Substring(root.Length) : path;
        }

        /// <summary>
        /// Returns the folder itself or its closest existing parent, or <paramref name="fallback"/> when none exists.
        /// </summary>
        public static string GetClosestExistingFolder(string absolutePath, string fallback)
        {
            var folder = absolutePath;

            while (!string.IsNullOrEmpty(folder))
            {
                if (Directory.Exists(folder)) return folder;

                folder = Path.GetDirectoryName(folder);
            }

            return fallback;
        }
    }
}
