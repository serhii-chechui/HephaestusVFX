using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace WTFGames.Hephaestus.VFX.Editor
{
    [CustomEditor(typeof(VFXLibraryConstants))]
    public class VFXLibraryConstantsEditor : UnityEditor.Editor
    {
        private const string EntityType = "VFX";

        private string _enumClassName = "VFXLibraryConstants";

        private string _newConstantKey = string.Empty;

        private SerializedProperty _enumsPathProperty;

        private SerializedProperty _keysProperty;

        private SerializedProperty _nextKeyIdProperty;

        private void OnEnable()
        {
            _enumsPathProperty = serializedObject.FindProperty(nameof(VFXLibraryConstants.enumsPath));
            _keysProperty = serializedObject.FindProperty(nameof(VFXLibraryConstants.keys));
            _nextKeyIdProperty = serializedObject.FindProperty(nameof(VFXLibraryConstants.nextKeyId));
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            var exportRequested = DrawExport();

            EditorGUILayout.Space();

            DrawAddKey();

            EditorGUILayout.Space();

            DrawKeys();

            serializedObject.ApplyModifiedProperties();

            if (exportRequested)
            {
                ExportKeysToEnum((VFXLibraryConstants)target);
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Save Config", GUILayout.ExpandWidth(true), GUILayout.Height(32))) {
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        private bool DrawExport()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.LabelField("Export:", EditorStyles.largeLabel);

            _enumClassName = EditorGUILayout.TextField("Enum Class Name:", _enumClassName);

            EditorGUILayout.BeginHorizontal();

            if (!string.IsNullOrEmpty(_enumsPathProperty.stringValue))
            {
                EditorGUILayout.LabelField("Path", _enumsPathProperty.stringValue, GUILayout.ExpandWidth(true));
            }

            if (GUILayout.Button("Pick", GUILayout.Width(96)))
            {
                var pickedPath = EditorUtility.OpenFolderPanel("Pick The Folder", GetAbsoluteEnumsPath(_enumsPathProperty.stringValue), "");

                // An empty path means the dialog was cancelled.
                if (!string.IsNullOrEmpty(pickedPath))
                {
                    _enumsPathProperty.stringValue = ToProjectRelativePath(pickedPath);
                }
            }

            EditorGUILayout.EndHorizontal();

            var exportErrors = GetExportErrors();

            foreach (var error in exportErrors)
            {
                EditorGUILayout.HelpBox(error, MessageType.Error);
            }

            GUI.enabled = exportErrors.Count == 0;
            var exportRequested = GUILayout.Button("Export to enum", GUILayout.ExpandWidth(true), GUILayout.Height(32));
            GUI.enabled = true;

            EditorGUILayout.EndVertical();

            return exportRequested;
        }

        private void DrawAddKey()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.LabelField($"Add New {EntityType} Keys:", EditorStyles.largeLabel);

            _newConstantKey = VFXIdentifierUtility.ToKey(EditorGUILayout.TextField("New Key:", _newConstantKey));

            var keyError = VFXKeysValidation.GetNewKeyError(_newConstantKey, GetKeyNames(), _nextKeyIdProperty.intValue);

            if (keyError != null && !string.IsNullOrEmpty(_newConstantKey))
            {
                EditorGUILayout.HelpBox(keyError, MessageType.Warning);
            }

            GUI.enabled = keyError == null;

            if (GUILayout.Button($"Add New {EntityType} Key", GUILayout.ExpandWidth(true), GUILayout.Height(32)))
            {
                var index = _keysProperty.arraySize;
                _keysProperty.InsertArrayElementAtIndex(index);

                var key = _keysProperty.GetArrayElementAtIndex(index);
                key.FindPropertyRelative(nameof(VFXConstantKey.name)).stringValue = _newConstantKey;
                key.FindPropertyRelative(nameof(VFXConstantKey.id)).intValue = _nextKeyIdProperty.intValue;

                _nextKeyIdProperty.intValue++;
                _newConstantKey = string.Empty;
                GUI.FocusControl(null);
            }

            GUI.enabled = true;

            EditorGUILayout.EndVertical();
        }

        private void DrawKeys()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.LabelField($"List of {EntityType} Keys:", EditorStyles.largeLabel);

            var removeIndex = -1;

            for (var i = 0; i < _keysProperty.arraySize; i++)
            {
                var key = _keysProperty.GetArrayElementAtIndex(i);
                var nameProperty = key.FindPropertyRelative(nameof(VFXConstantKey.name));
                var idProperty = key.FindPropertyRelative(nameof(VFXConstantKey.id));

                EditorGUILayout.BeginHorizontal();

                nameProperty.stringValue = VFXIdentifierUtility.ToKey(EditorGUILayout.TextField(nameProperty.stringValue));

                EditorGUILayout.LabelField(idProperty.intValue.ToString(), GUILayout.Width(32));

                if (GUILayout.Button("Remove", GUILayout.Width(64)))
                {
                    removeIndex = i;
                }

                EditorGUILayout.EndHorizontal();
            }

            // Remove after the loop so the remaining rows keep their layout this frame.
            if (removeIndex >= 0)
            {
                _keysProperty.DeleteArrayElementAtIndex(removeIndex);
            }

            foreach (var error in VFXKeysValidation.GetKeysErrors(GetKeyNames()))
            {
                EditorGUILayout.HelpBox(error, MessageType.Error);
            }

            EditorGUILayout.EndVertical();
        }

        private List<string> GetExportErrors()
        {
            var errors = VFXKeysValidation.GetKeysErrors(GetKeyNames());

            if (!VFXIdentifierUtility.IsIdentifier(_enumClassName))
            {
                errors.Add("Enum Class Name must be a valid C# identifier.");
            }

            if (string.IsNullOrEmpty(_enumsPathProperty.stringValue))
            {
                errors.Add("Pick the folder to export the enum to.");
            }
            else if (!Directory.Exists(GetAbsoluteEnumsPath(_enumsPathProperty.stringValue)))
            {
                errors.Add($"The folder {_enumsPathProperty.stringValue} doesn't exist.");
            }

            return errors;
        }

        private List<string> GetKeyNames()
        {
            var names = new List<string>();

            for (var i = 0; i < _keysProperty.arraySize; i++)
            {
                names.Add(_keysProperty.GetArrayElementAtIndex(i).FindPropertyRelative(nameof(VFXConstantKey.name)).stringValue);
            }

            return names;
        }

        private void ExportKeysToEnum(VFXLibraryConstants vfxLibraryConstants)
        {
            var namespaceName = VFXEnumGenerator.GetNamespace(Application.companyName, Application.productName, EntityType);
            var source = VFXEnumGenerator.Generate(namespaceName, _enumClassName, vfxLibraryConstants.keys);

            var filePath = Path.Combine(GetAbsoluteEnumsPath(vfxLibraryConstants.enumsPath), $"{_enumClassName}.cs");
            File.WriteAllText(filePath, source);

            AssetDatabase.Refresh();
        }

        private static string GetProjectRoot()
        {
            return Path.GetDirectoryName(Application.dataPath);
        }

        private static string GetAbsoluteEnumsPath(string enumsPath)
        {
            if (string.IsNullOrEmpty(enumsPath)) return string.Empty;

            return Path.IsPathRooted(enumsPath) ? enumsPath : Path.Combine(GetProjectRoot(), enumsPath);
        }

        /// <summary>
        /// Stores folders inside the project relative to its root, so the path works on every machine.
        /// </summary>
        private static string ToProjectRelativePath(string absolutePath)
        {
            var projectRoot = GetProjectRoot().Replace('\\', '/') + "/";
            var path = absolutePath.Replace('\\', '/');

            return path.StartsWith(projectRoot) ? path.Substring(projectRoot.Length) : path;
        }
    }
}
