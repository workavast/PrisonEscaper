using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum ConnectorId
{
    LeftUp,
    LeftDown,
    RightUp,
    RightDown,
}

public class RoomData : MonoBehaviour
{
    [Serializable]
    private struct ConnectorPointData
    {
        public ConnectorId id;
        public Transform point;
    }
    
    [SerializeField] private Transform leftUpSizePoint;
    [SerializeField] private Transform rightDownSizePoint;
    [SerializeField] private List<ConnectorPointData> connectorsData;
    [SerializeField] private List<Transform> enemySpawnPoints;
    [SerializeField] private List<Transform> lootBoxSpawnPoints;
    
    private Dictionary<ConnectorId, Transform> _connectors = new Dictionary<ConnectorId, Transform>();
    private Dictionary<ConnectorId, bool> _freeConnectors = new Dictionary<ConnectorId, bool>();
    
    public Dictionary<ConnectorId,Transform>.KeyCollection Connectors => _connectors.Keys;
    public Vector3 LeftSize => leftUpSizePoint.position;
    public Vector3 RightSize => rightDownSizePoint.position;

    public List<Transform> EnemySpawnPoints => enemySpawnPoints;

    private void Awake()
    {
        foreach (var connector in connectorsData)
        {
            if (_connectors.ContainsKey(connector.id))
                throw new Exception("Connector duplicate");
            
            _connectors.Add(connector.id,connector.point);
            
            _freeConnectors.Add(connector.id, true);
        }
        
    }
    
    public void TakeConnector(ConnectorId id)
    {
        if(_freeConnectors.ContainsKey(id))
            _freeConnectors[id] = false;
    }
    
    public Dictionary<Vector3, ConnectorId> GetFreeConnectorsPositions()
    {
        Dictionary<Vector3, ConnectorId> connectorsPositions = new Dictionary<Vector3, ConnectorId>();
        foreach (var connector in _freeConnectors)
        {
            if (connector.Value)
            {
                connectorsPositions.Add(_connectors[connector.Key].position, connector.Key);
            }
        }

        return connectorsPositions;
    }
    
    public bool GetConnectorPosition(ConnectorId id, out Vector3 position)
    {
        foreach (var connector in connectorsData)
        {
            if (connector.id == id)
            {
                position = connector.point.position;
                return true;
            }
        }

        position = new Vector3();
        return false;
    }
    
    public List<ConnectorId> GetFreeConnectors()
    {
        List<ConnectorId> connectorIds = new List<ConnectorId>();

        foreach (var connector in _freeConnectors)
        {
            if (connector.Value)
            {
                connectorIds.Add(connector.Key);
            }
        }

        return connectorIds;
    }
    
    public List<ConnectorId> GetAllConnectors()
    {
        List<ConnectorId> connectorIds = new List<ConnectorId>();

        foreach (var connector in connectorsData)
        {
            connectorIds.Add(connector.id);
        }

        return connectorIds;
    }

    public List<Vector3> GetFreeEnemiesPositions()
    {
        List<Vector3> points = new List<Vector3>();
        foreach (var enemySpawnPoint in enemySpawnPoints)
        {
            points.Add(enemySpawnPoint.position);
        }

        return points;
    }
    
    public List<Vector3> GetFreeLootBoxesPositions()
    {
        List<Vector3> points = new List<Vector3>();
        foreach (var lootBoxSpawnPoint in lootBoxSpawnPoints)
        {
            points.Add(lootBoxSpawnPoint.position);
        }

        return points;
    }
    
    [ContextMenu("Find size points")]
    public void FindSizePoints()
    {
        leftUpSizePoint = GameObject.Find("LeftUpSizePoint").transform;
        rightDownSizePoint = GameObject.Find("RightDownSizePoint").transform;
    }
}