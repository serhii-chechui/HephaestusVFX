using System;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace WTFGames.Hephaestus.VFX.Editor {
    [CustomEditor(typeof(VFXLibrary))]
    public class VFXLibraryEditor : UnityEditor.Editor {

        private SerializedProperty _vfxListProperty;

        private ReorderableList _reorderableList;

        private string[] _keyNames = Array.Empty<string>();

        private int[] _keyIds = Array.Empty<int>();

        private void OnEnable() {
            _vfxListProperty = serializedObject.FindProperty(nameof(VFXLibrary.vfxList));

            _reorderableList = new ReorderableList(serializedObject, _vfxListProperty, true, true, true, true) {
                drawHeaderCallback = DrawHeader,
                drawElementCallback = DrawElement
            };
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            serializedObject.Update();

            CacheKeys();

            // Actually draw the list in the inspector
            _reorderableList.DoLayoutList();

            DrawDuplicatesWarning();

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();

            if (GUILayout.Button("Save Library", GUILayout.ExpandWidth(true), GUILayout.Height(32f))) {
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
            }
        }

        private void CacheKeys() {
            var constants = ((VFXLibrary) target).widgetsLibraryConstants;

            if (constants == null) {
                _keyNames = Array.Empty<string>();
                _keyIds = Array.Empty<int>();
                EditorGUILayout.HelpBox($"Assign {nameof(VFXLibraryConstants)} to pick VFX keys.", MessageType.Warning);
                return;
            }

            _keyNames = constants.keys.Select(key => key.name).ToArray();
            _keyIds = constants.keys.Select(key => key.id).ToArray();
        }

        private void DrawDuplicatesWarning() {
            var duplicates = Enumerable.Range(0, _vfxListProperty.arraySize)
                .Select(i => _vfxListProperty.GetArrayElementAtIndex(i).FindPropertyRelative(nameof(VFXNamePair.vfxType)).intValue)
                .GroupBy(id => id)
                .Where(group => group.Count() > 1)
                .Select(group => GetKeyLabel(group.Key))
                .ToArray();

            if (duplicates.Length > 0) {
                EditorGUILayout.HelpBox($"Several prefabs are mapped to {string.Join(", ", duplicates)}; only the first one is used.", MessageType.Warning);
            }
        }

        /// <summary>
        /// Draws the header of the list
        /// </summary>
        private void DrawHeader(Rect rect) {
            GUI.Label(rect, "Dependencies between VFX name and Prefab based on Particles System.", EditorStyles.boldLabel);
        }

        /// <summary>
        /// Draws one element of the list
        /// </summary>
        private void DrawElement(Rect rect, int index, bool active, bool focused) {
            var element = _vfxListProperty.GetArrayElementAtIndex(index);
            var typeProperty = element.FindPropertyRelative(nameof(VFXNamePair.vfxType));
            var prefabProperty = element.FindPropertyRelative(nameof(VFXNamePair.vfxPrefab));

            var names = _keyNames;
            var ids = _keyIds;

            // Keep an entry whose key was removed visible instead of silently remapping it to another key.
            if (!ids.Contains(typeProperty.intValue)) {
                names = names.Append(GetKeyLabel(typeProperty.intValue)).ToArray();
                ids = ids.Append(typeProperty.intValue).ToArray();
            }

            typeProperty.intValue = EditorGUI.IntPopup(
                new Rect(rect.x, rect.y, rect.width * 0.5f - 40f, EditorGUIUtility.singleLineHeight),
                typeProperty.intValue,
                names,
                ids
            );

            EditorGUI.PropertyField(
                new Rect(rect.x + rect.width * 0.5f + 8f, rect.y, rect.width * 0.5f - 8f, EditorGUIUtility.singleLineHeight),
                prefabProperty,
                GUIContent.none
            );
        }

        private string GetKeyLabel(int id) {
            var index = Array.IndexOf(_keyIds, id);
            return index >= 0 ? _keyNames[index] : $"<Missing key {id}>";
        }
    }
}
