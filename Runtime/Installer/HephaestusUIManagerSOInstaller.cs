using UnityEngine;
using Zenject;

namespace WTFGames.Hephaestus.VFX
{
    [CreateAssetMenu(fileName = "HephaestusVFXManagerSOInstaller", menuName = "HephaestusMobile/Core/VFX/HephaestusVFXManagerSOInstaller")]
    public class HephaestusVFXManagerSOInstaller : ScriptableObjectInstaller
    {
        [SerializeField]
        private VFXManagerConfig vfxManagerConfig;
        
        public override void InstallBindings()
        {
            Container.BindInstance(vfxManagerConfig);
        }
    }
}