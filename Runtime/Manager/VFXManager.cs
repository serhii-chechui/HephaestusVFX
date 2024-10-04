using System;
using UnityEngine;
using Zenject;

namespace WTFGames.Hephaestus.VFX {
    public class VFXManager : IInitializable, IDisposable, IVFXManager {
        
        [Inject]
        private VFXManagerConfig _vfxManagerConfig;

        private VFXManagerHandler _vfxManagerHandler;
        
        /// <inheritdoc cref="IVFXManager"/>
        public void Initialize() {
            if (_vfxManagerHandler == null)
            {
                _vfxManagerHandler = new GameObject("_vfxManagerHandler").AddComponent<VFXManagerHandler>();
            }
            
            _vfxManagerHandler.Initialize(_vfxManagerConfig);
        }
        
        public void Dispose()
        {
            
        }

        /// <inheritdoc cref="IVFXManager"/>
        public void PlayVFX(Enum vfxName, Vector3 position = default, Transform parent = null) {
            _vfxManagerHandler.PlayVFX(vfxName, position, parent);
        }
    }
}