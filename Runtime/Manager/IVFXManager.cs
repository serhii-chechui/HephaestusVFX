using System;
using UnityEngine;

namespace WTFGames.Hephaestus.VFX {
    public interface IVFXManager {

        /// <summary>
        /// Initialize the VFX manager. Zenject calls it through IInitializable, so there is no need to call it manually.
        /// </summary>
        [Obsolete("Zenject initializes the VFX manager through IInitializable. This member will be removed from the interface in the next major version.")]
        void Initialize();

        /// <summary>
        /// Instantiate and play VFX entity.
        /// </summary>
        /// <param name="vfxName">VFX name.</param>
        /// <param name="position">Desired position in world space.</param>
        /// <param name="parent">Parent object.</param>
        void PlayVFX(Enum vfxName, Vector3 position = default, Transform parent = null);
    }
}
