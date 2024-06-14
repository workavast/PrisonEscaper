using SourceCode.Audio;
using UnityEngine;
using Zenject;

namespace GameCode.Scenes.Audio
{
    public class GameBootstrap : MonoBehaviour
    {
        [Inject] private readonly AudioVolumeChanger _audioVolumeChanger;

        private void Start()
        {
            _audioVolumeChanger.StartInit();
        }
    }
}