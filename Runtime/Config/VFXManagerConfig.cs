using UnityEngine;

namespace HephaestusMobile.VFXSystem.Config {
    [CreateAssetMenu(fileName = "VFXManagerConfig", menuName = "HephaestusMobile/Core/VFX/VFXManagerConfig")]
    public class VFXManagerConfig : ScriptableObject {
        public VFXLibrary vfxLibrary;
    }
}