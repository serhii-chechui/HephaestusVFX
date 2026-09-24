using System;
using UnityEngine;

namespace WTFGames.Hephaestus.VFX {
    public interface IVFXManager {

        /// <summary>
        /// Instantiate and play VFX entity.
        /// </summary>
        /// <param name="vfxName">VFX name.</param>
        /// <param name="position">Desired position in world space.</param>
        /// <param name="parent">Parent object.</param>
        void PlayVFX(Enum vfxName, Vector3 position = default, Transform parent = null);
    }
}
