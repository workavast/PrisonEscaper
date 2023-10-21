using UnityEngine;

namespace LevelGeneration
{
    public class StartBlockData : BlockData, IStartBlockData
    {
        [SerializeField] private Transform playerSpawnPosition;
        public Vector3 PlayerSpawnPosition => playerSpawnPosition.position;
    }

    public interface IStartBlockData : IBlockData
    {
        public Vector3 PlayerSpawnPosition { get; }
    }
}