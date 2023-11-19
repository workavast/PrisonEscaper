using Zenject;

namespace Installers
{
    public class VolumeProfileControllerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<VolumeProfileController>().FromInstance(FindObjectOfType<VolumeProfileController>(true)).AsSingle();
        }
    }
} 