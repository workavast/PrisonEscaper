using GameCycleFramework;
using Zenject;

namespace Core
{
    public class GameCycleControllerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            IGameCycleController gameCycleController = FindObjectOfType<GameCycleController>();
            Container.Bind<IGameCycleController>().FromInstance(gameCycleController).AsSingle();
        }
    }
}