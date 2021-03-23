using UnityEngine;
using HephaestusMobile.VFXSystem.Config;

namespace HephaestusMobile.VFXSystem.Manager {
    public class VFXManager : MonoBehaviour, IVFXManager {
        private VFXLibrary.VFXLibrary _vfxLibrary;
        
        /// <inheritdoc cref="IVFXManager"/>
        public void Initialize(VFXManagerConfig vfxManagerConfig) {
            _vfxLibrary = vfxManagerConfig.vfxLibrary;
        }

        /// <inheritdoc cref="IVFXManager"/>
        public void PlayVFX(string vfxName, Vector3 position, Transform parent = null) {
            var vfx = _vfxLibrary.vfxList.Find(v => v.vfxName == vfxName).vfxPrefab;
            var newVFX = Instantiate(vfx, parent, parent != null);
            newVFX.transform.position = position;
        }
    }
}