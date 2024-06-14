using GameCode.Scenes.Audio;
using SourceCode.Audio;
using SourceCode.Core.GlobalData;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

namespace GameCode.Scenes
{
    public class GameContextInstaller : MonoInstaller
    {
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private GameBootstrap gameBootstrapPrefab;
        
        public override void InstallBindings()
        {
            BindAudioVolumeChanger();
            BindFpsCapChanger();
            BindGameBootstrap();
        }

        private void BindAudioVolumeChanger()
        {
            Container.Bind<AudioVolumeChanger>().FromNew().AsSingle()
                .WithArguments(audioMixer, PlayerGlobalData.Instance.VolumeSettings).NonLazy();
        }
            
        private void BindFpsCapChanger()
        {
            Container.Bind<FpsCapChanger>().FromNew().AsSingle()
                .WithArguments(PlayerGlobalData.Instance.FpsSettings).NonLazy();
        }
        
        private void BindGameBootstrap()
        {
            Container.InstantiatePrefab(gameBootstrapPrefab);
        }
    }
}