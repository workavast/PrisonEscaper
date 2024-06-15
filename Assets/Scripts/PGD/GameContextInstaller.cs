using GameCode.audio;
using GameCode.Core;
using GameCode.Scenes.Audio;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

namespace GameCode.PGD
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