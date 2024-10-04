using System;
using UnityEngine;

namespace WTFGames.Hephaestus.VFX {
    public interface IVFXManager {
        
        /// <summary>
        /// Initialize the VFX manager.
        /// </summary>
        /// <param name="vfxManagerConfig">VFXManagerConfig file.</param>
        void Initialize();
        
        /// <summary>
        /// Insttantiate and play VFX entity.
        /// </summary>
        /// <param name="vfxName">VFX name.</param>
        /// <param name="position">Desired position.</param>
        /// <param name="parent">Parent object.</param>
        void PlayVFX(Enum vfxName, Vector3 position, Transform parent = null);
    }
}