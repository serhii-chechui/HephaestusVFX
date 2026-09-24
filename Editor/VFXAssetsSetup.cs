using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace WTFGames.Hephaestus.VFX.Editor
{
    /// <summary>
    /// Makes sure the project has the VFX assets and that they reference each other.
    /// Missing assets are created in <see cref="AssetsFolder"/>; existing ones are reused wherever they are.
    /// </summary>
    [InitializeOnLoad]
    public static class VFXAssetsSetup
    {
        public const string AssetsFolder = "Assets/Hephaestus/VFX";

        static VFXAssetsSetup()
        {
            // Don't change projects during batch mode builds.
            if (Application.isBatchMode) return;

            EditorApplication.delayCall += () =>
            {
                if (EditorApplication.isPlayingOrWillChangePlaymode) return;

                Run();
            };
        }

        [MenuItem("Hephaestus/VFX/Set Up Assets")]
        public static void Run()
        {
            SetUp(AssetsFolder);
        }

        /// <summary>
        /// Sets up the assets using <paramref name="folder"/> for the missing ones and returns what was changed.
        /// Existing assets are searched in <paramref name="searchInFolders"/>, or in the whole project when it is null.
        /// </summary>
        internal static List<string> SetUp(string folder, string[] searchInFolders = null)
        {
            var report = new List<string>();

            var constants = FindOrCreate<VFXLibraryConstants>(folder, searchInFolders, report);
            var library = FindOrCreate<VFXLibrary>(folder, searchInFolders, report);
            var config = FindOrCreate<VFXManagerConfig>(folder, searchInFolders, report);
            var installer = FindOrCreate<HephaestusVFXManagerSOInstaller>(folder, searchInFolders, report);

            SetPathIfEmpty(constants, nameof(VFXLibraryConstants.enumsPath), folder, report);
            LinkIfEmpty(library, nameof(VFXLibrary.vfxLibraryConstants), constants, report);
            LinkIfEmpty(config, nameof(VFXManagerConfig.vfxLibrary), library, report);
            LinkIfEmpty(installer, "vfxManagerConfig", config, report);

            if (report.Count == 0) return report;

            AssetDatabase.SaveAssets();
            Debug.Log($"[Hephaestus VFX] Assets set up:\n{string.Join("\n", report)}");

            return report;
        }

        /// <summary>
        /// Returns an existing asset of the type, preferring one in <paramref name="folder"/>, or creates it there.
        /// </summary>
        private static T FindOrCreate<T>(string folder, string[] searchInFolders, List<string> report) where T : ScriptableObject
        {
            var filter = $"t:{typeof(T).Name}";
            var existingFolders = searchInFolders?.Where(AssetDatabase.IsValidFolder).ToArray();

            // Search nothing, rather than the whole project, when none of the given folders exists.
            var guids = existingFolders == null
                ? AssetDatabase.FindAssets(filter)
                : existingFolders.Length > 0 ? AssetDatabase.FindAssets(filter, existingFolders) : new string[0];

            var paths = guids
                .Select(AssetDatabase.GUIDToAssetPath)
                .OrderByDescending(path => path.StartsWith(folder + "/"))
                .ToList();

            foreach (var path in paths)
            {
                var existing = AssetDatabase.LoadAssetAtPath<T>(path);

                if (existing != null) return existing;
            }

            EnsureFolder(folder);

            var asset = ScriptableObject.CreateInstance<T>();
            var assetPath = $"{folder}/{typeof(T).Name}.asset";
            AssetDatabase.CreateAsset(asset, assetPath);
            report.Add($"created {assetPath}");

            return asset;
        }

        private static void LinkIfEmpty(Object target, string propertyName, Object value, List<string> report)
        {
            var serializedObject = new SerializedObject(target);
            var property = serializedObject.FindProperty(propertyName);

            if (property.objectReferenceValue != null) return;

            property.objectReferenceValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            report.Add($"linked {target.name}.{propertyName} -> {value.name}");
        }

        private static void SetPathIfEmpty(Object target, string propertyName, string value, List<string> report)
        {
            var serializedObject = new SerializedObject(target);
            var property = serializedObject.FindProperty(propertyName);

            if (!string.IsNullOrEmpty(property.stringValue)) return;

            property.stringValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            report.Add($"set {target.name}.{propertyName} = {value}");
        }

        private static void EnsureFolder(string folder)
        {
            if (AssetDatabase.IsValidFolder(folder)) return;

            var separatorIndex = folder.LastIndexOf('/');
            var parent = folder.Substring(0, separatorIndex);

            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, folder.Substring(separatorIndex + 1));
        }
    }
}
