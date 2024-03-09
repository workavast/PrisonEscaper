using System.Collections.Generic;
using Enemies;
using UnityEngine;

namespace LevelGeneration
{
    public interface IBlockData
    {
        public Vector3 LeftUpSizePoint { get; }
        public Vector3 RightDownSizePoint { get; }
        public GameObject gameObject { get; }
        
        public IReadOnlyList<ConnectorID> AllConnectors { get; }
        public IReadOnlyList<ConnectorID> FreeConnectors { get; }
        public IReadOnlyDictionary<ConnectorID, Vector3> FreeConnectorsPositions { get; }

        public IReadOnlyList<SpawnableObjectCell<EnemyID>> EnemiesPoints { get; }
        public IReadOnlyList<SpawnableObjectCell<LootBoxID>> LootBoxesPoints { get; }
        public IReadOnlyList<SpawnableObjectCell<TrapID>> TrapsPoints { get; }

        public void OccupyConnector(ConnectorID id);
        public Vector3 GetConnectorPosition(ConnectorID id);
    }
}