using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LevelGeneration
{
    public class BlockDataPrototype
    {
        private readonly Transform _leftUpSizePoint;
        private readonly Transform _rightDownSizePoint;
        private readonly DictionaryInspector<ConnectorID, Transform> _connectorsData;
        private readonly Dictionary<ConnectorID, bool> _freeConnectors = new();

        public Vector3 Position { get; private set; }
        public BlockType BlockType { get; }
        public GameObject Prefab { get; }
        
        public Vector3 LeftUpSizePoint => _leftUpSizePoint.position + Position;
        public Vector3 RightDownSizePoint => _rightDownSizePoint.position + Position;

        public IReadOnlyList<ConnectorID> AllConnectors => _connectorsData.Select(data => data.Key).ToList();
        public IReadOnlyList<ConnectorID> FreeConnectors => (from connector in _freeConnectors where connector.Value select connector.Key).ToList();
        public IReadOnlyDictionary<ConnectorID, Vector3> FreeConnectorsPositions => _freeConnectors.Where(connector => connector.Value)
                .ToDictionary(connector => connector.Key, connector => _connectorsData[connector.Key].position + Position);
        
        public BlockDataPrototype(BlockType blockType, GameObject prefab, Transform leftUpSizePoint, Transform rightDownSizePoint, DictionaryInspector<ConnectorID, Transform> connectorsData)
        {
            BlockType = blockType;
            Prefab = prefab;
            _leftUpSizePoint = leftUpSizePoint;
            _rightDownSizePoint = rightDownSizePoint;
            _connectorsData = connectorsData;
            
            foreach (var connector in _connectorsData)
                _freeConnectors.Add(connector.Key, true);
        }

        public void SetPosition(Vector3 pos)
        {
            Position = pos;
        }
        
        public void OccupyConnector(ConnectorID id)
        {
            if (_freeConnectors.ContainsKey(id))
                _freeConnectors[id] = false;
        }

        public Vector3 GetConnectorPosition(ConnectorID id)
        {
            if (_connectorsData.ContainsKey(id))
                return _connectorsData[id].position + Position;
            
            throw new Exception($"Invalid ConnectorID: {id} in {this}");
        }
    }
}