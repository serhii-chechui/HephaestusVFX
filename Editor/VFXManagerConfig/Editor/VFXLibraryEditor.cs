using HephaestusMobile.VFXSystem.Config;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace HephaestusMobile.VFXSystem.Editor {
    [CustomEditor(typeof(VFXLibrary))]
    public class VFXLibraryEditor : UnityEditor.Editor {
        
        private ReorderableList _reorderableList;

        private VFXLibrary VFXLibrary => target as VFXLibrary;

        private void OnEnable() {
            if (VFXLibrary == null) return;

            _reorderableList = new ReorderableList(VFXLibrary.vfxList, typeof(VFXNamePair), true, true, true, true);

            // This could be used aswell, but I only advise this your class inherrits from UnityEngine.Object or has a CustomPropertyDrawer
            // Since you'll find your item using: serializedObject.FindProperty("list").GetArrayElementAtIndex(index).objectReferenceValue
            // which is a UnityEngine.Object
            // reorderableList = new ReorderableList(serializedObject, serializedObject.FindProperty("list"), true, true, true, true);

            // Add listeners to draw events
            _reorderableList.drawHeaderCallback += DrawHeader;
            _reorderableList.drawElementCallback += DrawElement;

            _reorderableList.onAddCallback += AddItem;
            _reorderableList.onRemoveCallback += RemoveItem;
        }

        private void OnDisable() {
            if (_reorderableList == null) return;

            // Make sure we don't get memory leaks etc.
            _reorderableList.drawHeaderCallback -= DrawHeader;
            _reorderableList.drawElementCallback -= DrawElement;

            _reorderableList.onAddCallback -= AddItem;
            _reorderableList.onRemoveCallback -= RemoveItem;
        }
        
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            if (_reorderableList == null) return;

            // Actually draw the list in the inspector
            _reorderableList.DoLayoutList();

            EditorGUILayout.Space();

            if (GUILayout.Button("Save Library", GUILayout.ExpandWidth(true), GUILayout.Height(32f))) {
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
            }
        }

        /// <summary>
        /// Draws the header of the list
        /// </summary>
        /// <param name="rect"></param>
        private void DrawHeader(Rect rect) {
            GUI.Label(rect, "Dependencies between VFX name and Prefab based on Particles System.", EditorStyles.boldLabel);
        }

        /// <summary>
        /// Draws one element of the list (ListItemExample)
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="index"></param>
        /// <param name="active"></param>
        /// <param name="focused"></param>
        private void DrawElement(Rect rect, int index, bool active, bool focused) {
            var item = VFXLibrary.vfxList[index];

            EditorGUI.BeginChangeCheck();

            item.vfxName = EditorGUI.TextField(new Rect(rect.x, rect.y, rect.width * 0.5f, EditorGUIUtility.singleLineHeight), item.vfxName);
            item.vfxPrefab = (GameObject) EditorGUI.ObjectField(new Rect(rect.x + rect.width * 0.5f + 8f, rect.y, rect.width * 0.5f - 8f, EditorGUIUtility.singleLineHeight), item.vfxPrefab, typeof(GameObject), false);

            if (EditorGUI.EndChangeCheck()) {
                EditorUtility.SetDirty(target);
            }

            // If you are using a custom PropertyDrawer, this is probably better
            // EditorGUI.PropertyField(rect, serializedObject.FindProperty("list").GetArrayElementAtIndex(index));
            // Although it is probably smart to cach the list as a private variable ;)
        }

        private void AddItem(ReorderableList list) {
            VFXLibrary.vfxList.Add(new VFXNamePair {vfxName = "NEW_VFX", vfxPrefab = null});
            EditorUtility.SetDirty(target);
        }

        private void RemoveItem(ReorderableList list) {
            VFXLibrary.vfxList.RemoveAt(list.index);
            EditorUtility.SetDirty(target);
        }
    }
}
