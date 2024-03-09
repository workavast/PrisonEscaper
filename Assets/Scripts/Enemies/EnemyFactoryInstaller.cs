using Zenject;

namespace Enemies
{
    public class EnemyFactoryInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            var factory = FindObjectOfType<EnemiesFactory>();
            Container.Bind<EnemiesFactory>().FromInstance(factory).AsSingle();
        }
    }
}