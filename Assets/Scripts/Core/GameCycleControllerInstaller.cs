using GameCycleFramework;
using Zenject;

namespace Core
{
    public class GameCycleControllerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            var gameCycleController = FindObjectOfType<GameCycleController>();
            
            Container.Bind<IGameCycleController>().FromInstance(gameCycleController).AsSingle();
            Container.Bind<IGameCycleSwitcher>().FromInstance(gameCycleController).AsSingle();
        }
    }
}