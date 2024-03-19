using Zenject;

namespace Projectiles
{
    public class ProjectilesRepositoryInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            var repository = FindObjectOfType<ProjectilesRepository>();
            Container.Bind<ProjectilesRepository>().FromInstance(repository).AsSingle();
        }
    }
}
