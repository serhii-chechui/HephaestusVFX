using System.Collections.Generic;
using UnityEngine;

namespace HephaestusMobile.VFXSystem.Config {
    [CreateAssetMenu(fileName = "VFXLibrary", menuName = "HephaestusMobile/Core/VFX/VFXLibrary")]
    public class VFXLibrary : ScriptableObject {
        [HideInInspector] public List<VFXNamePair> vfxList = new List<VFXNamePair>();
    }
}