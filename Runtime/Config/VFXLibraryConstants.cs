using System;
using System.Collections.Generic;
using UnityEngine;

namespace WTFGames.Hephaestus.VFX
{
    [CreateAssetMenu(fileName = "VFXLibraryConstants", menuName = "HephaestusMobile/Core/VFX/VFXLibraryConstants", order = 1)]
    public class VFXLibraryConstants : ScriptableObject, ISerializationCallbackReceiver
    {
        [HideInInspector]
        public string enumsPath;

        [HideInInspector]
        public List<VFXConstantKey> keys = new List<VFXConstantKey>();

        /// <summary>
        /// Id for the next added key. Ids are never reused, so a removed key can't silently take over its library mappings.
        /// </summary>
        [HideInInspector]
        public int nextKeyId;

        // Keys stored before stable ids were introduced; the id of each key was its index in this list.
        [SerializeField, HideInInspector]
        private List<string> uiMapKeys = new List<string>();

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            if (keys == null)
            {
                keys = new List<VFXConstantKey>();
            }

            if (uiMapKeys != null && uiMapKeys.Count > 0)
            {
                if (keys.Count == 0)
                {
                    for (var i = 0; i < uiMapKeys.Count; i++)
                    {
                        keys.Add(new VFXConstantKey { name = uiMapKeys[i], id = i });
                    }
                }

                uiMapKeys.Clear();
            }

            foreach (var key in keys)
            {
                nextKeyId = Math.Max(nextKeyId, key.id + 1);
            }
        }
    }
}
