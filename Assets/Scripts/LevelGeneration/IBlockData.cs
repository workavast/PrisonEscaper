using System.Collections.Generic;
using Enemies;
using UnityEngine;

namespace LevelGeneration
{
    public interface IBlockData
    {
        public GameObject gameObject { get; }
        public Vector3 LeftUpSizePoint { get; }
        public Vector3 RightDownSizePoint { get; }
        
        public IReadOnlyList<ConnectorID> AllConnectors { get; }
        public IReadOnlyDictionary<ConnectorID, Vector3> ConnectorsPositions { get; }

        public IReadOnlyList<SpawnableObjectCell<EnemyID>> EnemiesPoints { get; }
        public IReadOnlyList<SpawnableObjectCell<LootBoxID>> LootBoxesPoints { get; }
        public IReadOnlyList<SpawnableObjectCell<TrapID>> TrapsPoints { get; }

        public Vector3 GetConnectorPosition(ConnectorID id);

        public BlockDataPrototype GetPreparingBlock();
    }
}