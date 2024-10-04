using System;
using System.Collections.Generic;
using UnityEngine;

namespace WTFGames.Hephaestus.VFX {
    [CreateAssetMenu(fileName = "VFXLibrary", menuName = "HephaestusMobile/Core/VFX/VFXLibrary")]
    public class VFXLibrary : ScriptableObject {
        
        public VFXLibraryConstants widgetsLibraryConstants;
        
        [HideInInspector]
        public List<VFXNamePair> vfxList = new List<VFXNamePair>();
        
        public GameObject GetPrefabByType(Enum vfxType) {
            return vfxList.Find(w => w.vfxType == Convert.ToInt32(vfxType)).vfxPrefab;
        }
    }
}