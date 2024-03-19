using LevelGeneration.LevelsGenerators;
using UnityEngine;
using Zenject;

namespace LevelGeneration
{
    public class LevelGeneratorInstaller : MonoInstaller
    {
        [SerializeField] private LevelGeneratorBase levelGenerator;
        
        public override void InstallBindings()
        {
            Container.Bind<LevelGeneratorBase>().FromInstance(levelGenerator).AsSingle();
        }
    }
}