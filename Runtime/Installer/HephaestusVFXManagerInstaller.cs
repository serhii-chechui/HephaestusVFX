using Zenject;

namespace WTFGames.Hephaestus.VFX
{
    public class HephaestusVFXManagerInstaller : Installer<HephaestusVFXManagerInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<VFXManager>().AsSingle();
        }
    }
}