using System;
using System.Collections.Generic;
using UnityEngine;

namespace WTFGames.Hephaestus.VFX {
    [CreateAssetMenu(fileName = "VFXLibrary", menuName = "HephaestusMobile/Core/VFX/VFXLibrary")]
    public class VFXLibrary : ScriptableObject {

        public VFXLibraryConstants widgetsLibraryConstants;

        [HideInInspector]
        public List<VFXNamePair> vfxList = new List<VFXNamePair>();

        /// <summary>
        /// Returns the prefab mapped to the given VFX type, or null when there is no mapping.
        /// </summary>
        public GameObject GetPrefabByType(Enum vfxType) {
            var vfxTypeId = Convert.ToInt32(vfxType);
            return vfxList.Find(w => w.vfxType == vfxTypeId)?.vfxPrefab;
        }
    }
}
