using Zenject;

namespace Installers
{
    public class KeyboardObserverInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<KeyboardObserver>().FromInstance(FindObjectOfType<KeyboardObserver>())
                .AsSingle();
        }
    }
}