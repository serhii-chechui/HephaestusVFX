using System;
using UnityEngine;

namespace WTFGames.Hephaestus.VFX
{
    public class VFXManagerHandler : MonoBehaviour
    {
        private VFXLibrary _vfxLibrary;

        public void Initialize(VFXManagerConfig vfxManagerConfig)
        {
            _vfxLibrary = vfxManagerConfig.vfxLibrary;
        }

        public void Dismiss()
        {
            _vfxLibrary = null;
        }

        public void PlayVFX(Enum vfxName, Vector3 position = default, Transform parent = null)
        {
            if (_vfxLibrary == null)
            {
                Debug.LogError($"[{nameof(VFXManagerHandler)}] VFX library is not set, can't play {vfxName}.");
                return;
            }

            var vfx = _vfxLibrary.GetPrefabByType(vfxName);

            if (vfx == null)
            {
                Debug.LogWarning($"[{nameof(VFXManagerHandler)}] No prefab is mapped to {vfxName}.");
                return;
            }

            // The position is in world space; the prefab keeps its own rotation.
            var newVFX = Instantiate(vfx, position, vfx.transform.rotation, parent);

            var lifetime = GetLifetime(newVFX);

            if (lifetime > 0f)
            {
                Destroy(newVFX, lifetime);
            }
        }

        /// <summary>
        /// Returns how long the effect lives, or 0 when it has no particle systems or any of them loops.
        /// </summary>
        private static float GetLifetime(GameObject vfx)
        {
            var particleSystems = vfx.GetComponentsInChildren<ParticleSystem>(true);
            var lifetime = 0f;

            foreach (var particleSystem in particleSystems)
            {
                var main = particleSystem.main;

                if (main.loop)
                {
                    return 0f;
                }

                var duration = main.startDelay.constantMax + main.duration + main.startLifetime.constantMax;
                lifetime = Mathf.Max(lifetime, duration / Mathf.Max(main.simulationSpeed, Mathf.Epsilon));
            }

            return lifetime;
        }
    }
}
