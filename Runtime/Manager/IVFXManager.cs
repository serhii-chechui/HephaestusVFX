using UnityEngine;
using HephaestusMobile.VFXSystem.Config;

namespace HephaestusMobile.VFXSystem.Manager {
    public interface IVFXManager {
        
        /// <summary>
        /// Initialize the VFX manager.
        /// </summary>
        /// <param name="vfxManagerConfig">VFXManagerConfig file.</param>
        void Initialize(VFXManagerConfig vfxManagerConfig);
        
        /// <summary>
        /// Insttantiate and play VFX entity.
        /// </summary>
        /// <param name="vfxName">VFX name.</param>
        /// <param name="position">Desired position.</param>
        /// <param name="parent">Parent object.</param>
        void PlayVFX(string vfxName, Vector3 position, Transform parent = null);
    }
}