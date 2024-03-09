using Zenject;

namespace Enemies
{
    public class EnemiesRepositoryInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            var enemiesRepository = FindObjectOfType<EnemiesRepository>();
            Container.Bind<EnemiesRepository>().FromInstance(enemiesRepository).AsSingle();
        }
    }
}
