using System;
using Zenject;

namespace GameCode.Core.SceneLoading
{
    public class SceneLoaderInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            var sceneLoader = FindObjectOfType<SceneLoader>();
            if (sceneLoader == null)
                throw new NullReferenceException($"sceneLoader is null");
            Container.BindInstance(sceneLoader).AsSingle();
        }
    }
}