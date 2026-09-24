using System;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace WTFGames.Hephaestus.VFX {
    public class VFXManager : IInitializable, IDisposable, IVFXManager {

        [Inject]
        private VFXManagerConfig _vfxManagerConfig;

        private VFXManagerHandler _vfxManagerHandler;

        /// <summary>
        /// Creates the VFX handler. Zenject calls it through IInitializable.
        /// </summary>
        public void Initialize() {
            if (_vfxManagerHandler != null) return;

            var handlerObject = new GameObject("_vfxManagerHandler");
            Object.DontDestroyOnLoad(handlerObject);

            _vfxManagerHandler = handlerObject.AddComponent<VFXManagerHandler>();
            _vfxManagerHandler.Initialize(_vfxManagerConfig);
        }

        public void Dispose()
        {
            if (_vfxManagerHandler == null) return;

            _vfxManagerHandler.Dismiss();
            Object.Destroy(_vfxManagerHandler.gameObject);
            _vfxManagerHandler = null;
        }

        /// <inheritdoc cref="IVFXManager.PlayVFX"/>
        public void PlayVFX(Enum vfxName, Vector3 position = default, Transform parent = null) {
            if (_vfxManagerHandler == null)
            {
                Debug.LogError($"[{nameof(VFXManager)}] The manager is not initialized, can't play {vfxName}.");
                return;
            }

            _vfxManagerHandler.PlayVFX(vfxName, position, parent);
        }
    }
}
