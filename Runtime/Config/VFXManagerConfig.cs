using UnityEngine;

namespace WTFGames.Hephaestus.VFX {
    [CreateAssetMenu(fileName = "VFXManagerConfig", menuName = "HephaestusMobile/Core/VFX/VFXManagerConfig")]
    public class VFXManagerConfig : ScriptableObject {
        public VFXLibrary vfxLibrary;
    }
}