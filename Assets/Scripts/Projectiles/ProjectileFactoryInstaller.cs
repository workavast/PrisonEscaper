using Zenject;

namespace Projectiles
{
    public class ProjectileFactoryInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            var factory = FindObjectOfType<ProjectileFactory>();
            Container.Bind<ProjectileFactory>().FromInstance(factory).AsSingle();
        }
    }
}