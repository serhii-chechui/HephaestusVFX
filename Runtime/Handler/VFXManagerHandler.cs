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
            
        }

        public void PlayVFX(Enum vfxName, Vector3 position = default, Transform parent = null)
        {
            var vfx = _vfxLibrary.GetPrefabByType(vfxName);
            var newVFX = Instantiate(vfx, parent, parent != null);
            newVFX.transform.position = position;
        }
    }
}