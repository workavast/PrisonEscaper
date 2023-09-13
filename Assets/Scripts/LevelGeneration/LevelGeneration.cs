using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelGeneration : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    
    [Space]
    [SerializeField] private Transform roomsParent;
    [SerializeField] private Transform enemiesParent;
    [SerializeField] private Transform lootBoxesParent;
    
    [SerializeField] [Range(0, 100)] private int minRoomCount;
    [SerializeField] [Range(0, 100)] private int maxRoomCount;
    [Space] 
    [SerializeField] [Range(0, 100)] private int minPercentEnemies;
    [SerializeField] [Range(0, 100)] private int maxPercentEnemies;
    [Space]
    [SerializeField] [Range(0, 100)] private int minPercentLootBoxes;
    [SerializeField] [Range(0, 100)] private int maxPercentLootBoxes;
    
    [Space]
    [SerializeField] private GameObject startRoom;
    [SerializeField] private List<GameObject> finalRooms;
    [SerializeField] private List<GameObject> nRooms;
    [SerializeField] private List<GameObject> someRooms;
    [SerializeField] private List<GameObject> throwRooms;
    [SerializeField] private List<GameObject> endRooms;
    [SerializeField] private List<GameObject> shortEnds;
    [SerializeField] private List<GameObject> wallEnds;
    [Space]
    [SerializeField] private List<GameObject> enemiesPrefabs;
    [SerializeField] private List<GameObject> lootBoxesPrefabs;

    [Space]
    [SerializeField] private bool test;

    private Vector3 _playerSpawnPosition;
    private GameObject _player;

    private Dictionary<ConnectorId, List<GameObject>> _pairsConnectorRooms = new Dictionary<ConnectorId, List<GameObject>>()
        {
            {ConnectorId.LeftUp, new List<GameObject>()},
            {ConnectorId.LeftDown, new List<GameObject>()},
            {ConnectorId.RightUp, new List<GameObject>()},
            {ConnectorId.RightDown, new List<GameObject>()},
        };

    
    private List<RoomData> _spawnedRooms = new List<RoomData>();
    private List<RoomData> _roomsWithFreeConnectors = new List<RoomData>();

    private List<Vector3> _enemiesPositions = new List<Vector3>();
    private List<Vector3> _lootBoxesPositions = new List<Vector3>();
    
    private List<GameObject> _spawnedEnemies = new List<GameObject>();
    private List<GameObject> _spawnedLootBoxes = new List<GameObject>();
    
    private int iteration = 0;
    private bool finalRoomSpawned = false;

    private void Awake()
    {
        GenerateLevel();
        GenerateEnemies();
        GenerateLootBoxes();    
    }

    private void Update()
    {
        if (test && Input.GetKeyDown(KeyCode.R))
        {
            ClearLevel();
            GenerateLevel();
            GenerateEnemies();
            GenerateLootBoxes();
        }
    }
    
    private void ClearLevel()
    {
        if(GlobalLampBlinks.Instance)
            GlobalLampBlinks.Instance.Clear();
        
        foreach (var room in _spawnedRooms)
            Destroy(room.gameObject);
            
        foreach (var room in _roomsWithFreeConnectors)
            Destroy(room.gameObject);

        foreach (var enemy in _spawnedEnemies)
            Destroy(enemy);

        foreach (var lootBox in _spawnedLootBoxes)
            Destroy(lootBox);
        
        _spawnedRooms = new List<RoomData>();
        _roomsWithFreeConnectors = new List<RoomData>();
        _spawnedEnemies = new List<GameObject>();
        _spawnedLootBoxes = new List<GameObject>();
        _enemiesPositions = new List<Vector3>();
        _lootBoxesPositions = new List<Vector3>();
        finalRoomSpawned = false;
        iteration = 0;
    }
    
    
    private void GenerateLevel()
    {
        List<List<GameObject>> allRooms = new List<List<GameObject>>()
        {
            nRooms,
            someRooms,
            throwRooms
        };
        foreach (var rooms in allRooms)
        {
            foreach (var room in rooms)
            {
                List<ConnectorId> connectorIds = room.GetComponent<RoomData>().GetAllConnectors();
                foreach (var connector in connectorIds)
                {
                    _pairsConnectorRooms.TryGetValue(connector, out List<GameObject> pairsConnectorRooms);
                    pairsConnectorRooms?.Add(room);
                    _pairsConnectorRooms[connector] = pairsConnectorRooms;
                }
            }
        }
        
        CreateStartRoom();
        CreateMainRooms();

        if (!CreateFinalRoom(finalRooms))
        {
            ClearLevel();
            GenerateLevel();
            return;
        }
        
        CreateEndRooms(endRooms);
        CreateEndRooms(shortEnds);
        CreateEndRooms(wallEnds);

        if(_player == null)
            _player = Instantiate(playerPrefab, _playerSpawnPosition, Quaternion.Euler(0,0,0));
        else
            _player.transform.position = _playerSpawnPosition;
    }

    
    #region GenerateLevel

    private void CreateStartRoom()
    {
        if (!Instantiate(startRoom, roomsParent).TryGetComponent(out StartRoomData startRoomData))
            throw new Exception("Start room dont have script StartRoomData");
        
        _playerSpawnPosition = startRoomData.PlayerSpawnPosition;
        _spawnedRooms.Add(startRoomData);
        _roomsWithFreeConnectors.Add(startRoomData);
    }

    private void CreateMainRooms()
    {
        int roomsCount = Random.Range(minRoomCount, maxRoomCount);

        while (_spawnedRooms.Count < roomsCount)
        {
            iteration++;
            if (iteration > 250)
            {
                Debug.Log("TOO MUCH");
                if (_spawnedRooms.Count < roomsCount)
                {
                    ClearLevel();
                    CreateStartRoom();
                    CreateMainRooms();
                }
                    
                return;
            }
            
            int roomWithFreeConnectorsIndex = Random.Range(0, _roomsWithFreeConnectors.Count);
            RoomData chosenRoomWithFreeConnectors = _roomsWithFreeConnectors[roomWithFreeConnectorsIndex];
            List<ConnectorId> freeConnectorsOfChosenRoom = chosenRoomWithFreeConnectors.GetFreeConnectors();

            int randomConnectorIndex = Random.Range(0, freeConnectorsOfChosenRoom.Count);

            ConnectorId chosenNewRoomConnector;
            List<GameObject> possibleRooms = new List<GameObject>();
            
            
            
            if (!chosenRoomWithFreeConnectors.GetConnectorPosition(freeConnectorsOfChosenRoom[randomConnectorIndex],
                    out Vector3 currentFreeConnectorPosition))
                throw new Exception("Connector dont have position");

            

            switch (freeConnectorsOfChosenRoom[randomConnectorIndex])
            {
                case ConnectorId.LeftUp:
                {
                    int n = Random.Range(0, 2);
                    if(n == 1)
                        chosenNewRoomConnector = ConnectorId.RightUp;
                    else
                        chosenNewRoomConnector = ConnectorId.RightDown;

                    possibleRooms = _pairsConnectorRooms[chosenNewRoomConnector];
                    break;
                }
                case ConnectorId.LeftDown:
                {
                    int n = Random.Range(0, 2);
                    if(n == 1)
                        chosenNewRoomConnector = ConnectorId.RightUp;
                    else
                        chosenNewRoomConnector = ConnectorId.RightDown;

                    possibleRooms = _pairsConnectorRooms[chosenNewRoomConnector];
                    break;
                }
                case ConnectorId.RightUp:
                {
                    int n = Random.Range(0, 2);
                    if(n == 1)
                        chosenNewRoomConnector = ConnectorId.LeftUp;
                    else
                        chosenNewRoomConnector = ConnectorId.LeftDown;

                    possibleRooms = _pairsConnectorRooms[chosenNewRoomConnector];
                    break;
                }
                case ConnectorId.RightDown:
                {
                    int n = Random.Range(0, 2);
                    if(n == 1)
                        chosenNewRoomConnector = ConnectorId.LeftUp;
                    else
                        chosenNewRoomConnector = ConnectorId.LeftDown;

                    possibleRooms = _pairsConnectorRooms[chosenNewRoomConnector];
                    break;
                }
                default:
                {
                    throw new Exception("Un correct connectorId");
                }
            }

            
            if (possibleRooms.Count == 0)
                throw new Exception("possibleRooms.Count == 0");
            
            
            
            int possibleRoomIndex = Random.Range(0, possibleRooms.Count);
            GameObject chosenNewRoom = possibleRooms[possibleRoomIndex];
            RoomData chosenNewRoomData = chosenNewRoom.GetComponent<RoomData>();
            Vector3 leftSize = chosenNewRoomData.LeftSize;
            Vector3 rightSize = chosenNewRoomData.RightSize;
            
            
            if (!chosenNewRoomData.GetConnectorPosition(chosenNewRoomConnector, out Vector3 connectorPositionNewRoom))
                throw new Exception("Connector dont have position");


            Vector3 leftUpPosition = currentFreeConnectorPosition - connectorPositionNewRoom + leftSize;
            Vector3 rightDownPosition = currentFreeConnectorPosition - connectorPositionNewRoom + rightSize;
            
            if(CheckIntersection(leftUpPosition, rightDownPosition)) continue;
            
            GameObject newRoom = Instantiate(possibleRooms[possibleRoomIndex], roomsParent);
            RoomData newRoomDataSpawned = newRoom.GetComponent<RoomData>();
            newRoom.transform.position = currentFreeConnectorPosition -  connectorPositionNewRoom;
            chosenRoomWithFreeConnectors.TakeConnector(freeConnectorsOfChosenRoom[randomConnectorIndex]);
            newRoomDataSpawned.TakeConnector(chosenNewRoomConnector);
            _enemiesPositions.AddRange(newRoomDataSpawned.GetFreeEnemiesPositions());
            _lootBoxesPositions.AddRange(newRoomDataSpawned.GetFreeLootBoxesPositions());
            
            if (chosenRoomWithFreeConnectors.GetFreeConnectors().Count <= 0)
            {
                _roomsWithFreeConnectors.Remove(chosenRoomWithFreeConnectors);
            }
                
            _spawnedRooms.Add(newRoomDataSpawned);
            if (newRoomDataSpawned.GetFreeConnectors().Count > 0)
            {
                _roomsWithFreeConnectors.Add(newRoomDataSpawned);
            }


            for (int i = 0; i < _roomsWithFreeConnectors.Count; i++)
            {
                Dictionary<Vector3, ConnectorId> mainRoomConnectors = _roomsWithFreeConnectors[i].GetFreeConnectorsPositions();
                for (int n = i+1; n < _roomsWithFreeConnectors.Count; n++)
                {
                    Dictionary<Vector3, ConnectorId> curRoomConnectors = _roomsWithFreeConnectors[n].GetFreeConnectorsPositions();

                    foreach (var mainRoomConnector in mainRoomConnectors)
                    {
                        foreach (var curRoomConnector in curRoomConnectors)
                        {
                            if (Mathf.Abs(Vector3.Distance(mainRoomConnector.Key, curRoomConnector.Key)) < 0.1f)
                            {
                                _roomsWithFreeConnectors[i].TakeConnector(mainRoomConnector.Value);
                                _roomsWithFreeConnectors[n].TakeConnector(curRoomConnector.Value);
                            }
                        }
                    }
                        
                    if (_roomsWithFreeConnectors[i].GetFreeConnectors().Count <= 0)
                    {
                        _roomsWithFreeConnectors.Remove(_roomsWithFreeConnectors[i]);
                        i--;
                        n--;
                    }   
                                                                        
                    if (_roomsWithFreeConnectors[n].GetFreeConnectors().Count <= 0)
                    {
                        _roomsWithFreeConnectors.Remove(_roomsWithFreeConnectors[n]);
                        n--;
                    } 
                }
            }
        }
    }

    private bool CreateFinalRoom(List<GameObject> rooms)
    {
        if(finalRoomSpawned) return false;
        
        var connectorsDistances = new List<KeyValuePair<float, KeyValuePair<RoomData, ConnectorId>>>();
        foreach (var room in _roomsWithFreeConnectors)
        {
            Dictionary<Vector3,ConnectorId> connectorPositions = room.GetFreeConnectorsPositions();

            foreach (var connector in connectorPositions)
            {
                float newDistance = Mathf.Abs(Vector3.Distance(startRoom.transform.position, connector.Key));

                var newConnectors = new List<KeyValuePair<float, KeyValuePair<RoomData, ConnectorId>>>();


                if (connectorsDistances.Count <= 0)
                {
                    var newPair = new KeyValuePair<float, KeyValuePair<RoomData, ConnectorId>>(newDistance, new KeyValuePair<RoomData,ConnectorId>(room,connector.Value));
                    newConnectors.Add(newPair); 
                    newDistance = 0;
                }
                
                foreach (var connectorDistance in connectorsDistances)
                {
                    if (connectorDistance.Key >= newDistance)
                    {
                        newConnectors.Add(connectorDistance);
                    }
                    else
                    {
                        var newPair = new KeyValuePair<float, KeyValuePair<RoomData, ConnectorId>>(newDistance, new KeyValuePair<RoomData,ConnectorId>(room,connector.Value));
                        newConnectors.Add(newPair);
                        newConnectors.Add(connectorDistance);
                        newDistance = 0;
                    }
                }

                if (newDistance != 0)
                {
                    var newPair = new KeyValuePair<float, KeyValuePair<RoomData, ConnectorId>>(newDistance, new KeyValuePair<RoomData,ConnectorId>(room,connector.Value));
                    newConnectors.Add(newPair);
                }
                
                connectorsDistances = newConnectors;
            }
        }
        
        
        
        
        Dictionary<ConnectorId, List<GameObject>> pairsConnectorRooms = new Dictionary<ConnectorId, List<GameObject>>()
        {
            {ConnectorId.LeftUp, new List<GameObject>()},
            {ConnectorId.LeftDown, new List<GameObject>()},
            {ConnectorId.RightUp, new List<GameObject>()},
            {ConnectorId.RightDown, new List<GameObject>()},
        };
        
        foreach (var room in rooms)
        {
            List<ConnectorId> connectorIds = room.GetComponent<RoomData>().GetAllConnectors();
            foreach (var connector in connectorIds)
            {
                pairsConnectorRooms.TryGetValue(connector, out List<GameObject> pairs);
                pairs?.Add(room);
                pairsConnectorRooms[connector] = pairs;
            }
        }

        foreach (var connectorDistance in connectorsDistances)
        {
            RoomData chosenRoomWithFreeConnectors = connectorDistance.Value.Key;
            ConnectorId freeConnector = connectorDistance.Value.Value;
            
            if (!chosenRoomWithFreeConnectors.GetConnectorPosition(freeConnector, out Vector3 currentFreeConnectorPosition))
                throw new Exception("Connector dont have position");

            ConnectorId chosenNewRoomConnector;
            List<GameObject> possibleRooms;
            switch (freeConnector)
            {
                case ConnectorId.LeftUp:
                {
                    chosenNewRoomConnector = ConnectorId.RightUp;
                    possibleRooms = pairsConnectorRooms[chosenNewRoomConnector];
                    break;
                }
                case ConnectorId.LeftDown:
                {
                    chosenNewRoomConnector = ConnectorId.RightUp;
                    possibleRooms = pairsConnectorRooms[chosenNewRoomConnector];
                    break;
                }
                case ConnectorId.RightUp:
                {
                    chosenNewRoomConnector = ConnectorId.LeftUp;
                    possibleRooms = pairsConnectorRooms[chosenNewRoomConnector];
                    break;
                }
                case ConnectorId.RightDown:
                {
                    chosenNewRoomConnector = ConnectorId.LeftUp;
                    possibleRooms = pairsConnectorRooms[chosenNewRoomConnector];
                    break;
                }
                default:
                {
                    throw new Exception("Un correct connectorId");
                }
            }

            if (possibleRooms.Count == 0)
                throw new Exception("possibleRooms.Count == 0");
            
            int possibleRoomIndex = Random.Range(0, possibleRooms.Count);
            GameObject chosenNewRoom = possibleRooms[possibleRoomIndex];
            RoomData chosenNewRoomData = chosenNewRoom.GetComponent<RoomData>();

            if (!chosenNewRoomData.GetConnectorPosition(chosenNewRoomConnector, out Vector3 connectorPositionNewRoom))
                throw new Exception("Connector dont have position");
                
            Vector3 leftUpPosition = currentFreeConnectorPosition - connectorPositionNewRoom + chosenNewRoomData.LeftSize;
            Vector3 rightDownPosition = currentFreeConnectorPosition - connectorPositionNewRoom + chosenNewRoomData.RightSize;
            
            if(CheckIntersection(leftUpPosition, rightDownPosition)) continue;

            GameObject newRoom = Instantiate(possibleRooms[possibleRoomIndex], roomsParent);
            RoomData newRoomDataSpawned = newRoom.GetComponent<RoomData>();
            newRoom.transform.position = currentFreeConnectorPosition -  connectorPositionNewRoom;
            chosenRoomWithFreeConnectors.TakeConnector(freeConnector);
            newRoomDataSpawned.TakeConnector(chosenNewRoomConnector);
            _enemiesPositions.AddRange(newRoomDataSpawned.GetFreeEnemiesPositions());
            _lootBoxesPositions.AddRange(newRoomDataSpawned.GetFreeLootBoxesPositions());

            _spawnedRooms.Add(newRoomDataSpawned);
                
            if (chosenRoomWithFreeConnectors.GetFreeConnectors().Count <= 0)
            {
                _roomsWithFreeConnectors.Remove(chosenRoomWithFreeConnectors);
            }

            finalRoomSpawned = true;
            return true;
        }

        return false; //throw new Exception("dont find correct place");
    }
    
    private void CreateEndRooms(List<GameObject> rooms)
    { 
        Dictionary<ConnectorId, List<GameObject>> pairsConnectorRooms = new Dictionary<ConnectorId, List<GameObject>>()
        {
            {ConnectorId.LeftUp, new List<GameObject>()},
            {ConnectorId.LeftDown, new List<GameObject>()},
            {ConnectorId.RightUp, new List<GameObject>()},
            {ConnectorId.RightDown, new List<GameObject>()},
        };
        
        foreach (var room in rooms)
        {
            List<ConnectorId> connectorIds = room.GetComponent<RoomData>().GetAllConnectors();
            foreach (var connector in connectorIds)
            {
                pairsConnectorRooms.TryGetValue(connector, out List<GameObject> pairs);
                pairs?.Add(room);
                pairsConnectorRooms[connector] = pairs;
            }
        }

        int countIter = _roomsWithFreeConnectors.Count;
        for (int n = 0; n < countIter; n++)
        {
            RoomData chosenRoomWithFreeConnectors = _roomsWithFreeConnectors.First();

            List<ConnectorId> freeConnectors = chosenRoomWithFreeConnectors.GetFreeConnectors();
            
            foreach (var connector in freeConnectors)
            {
                if (!chosenRoomWithFreeConnectors.GetConnectorPosition(connector, out Vector3 currentFreeConnectorPosition))
                    throw new Exception("Connector dont have position");

                ConnectorId chosenNewRoomConnector;
                List<GameObject> possibleRooms;
                switch (connector)
                {
                    case ConnectorId.LeftUp:
                    {
                        chosenNewRoomConnector = ConnectorId.RightUp;
                        possibleRooms = pairsConnectorRooms[chosenNewRoomConnector];
                        break;
                    }
                    case ConnectorId.LeftDown:
                    {
                        chosenNewRoomConnector = ConnectorId.RightUp;
                        possibleRooms = pairsConnectorRooms[chosenNewRoomConnector];
                        break;
                    }
                    case ConnectorId.RightUp:
                    {
                        chosenNewRoomConnector = ConnectorId.LeftUp;
                        possibleRooms = pairsConnectorRooms[chosenNewRoomConnector];
                        break;
                    }
                    case ConnectorId.RightDown:
                    {
                        chosenNewRoomConnector = ConnectorId.LeftUp;
                        possibleRooms = pairsConnectorRooms[chosenNewRoomConnector];
                        break;
                    }
                    default:
                    {
                        throw new Exception("Un correct connectorId");
                    }
                }

                if (possibleRooms.Count == 0)
                    throw new Exception("possibleRooms.Count == 0");
            
            
                int possibleRoomIndex = Random.Range(0, possibleRooms.Count);
                GameObject chosenNewRoom = possibleRooms[possibleRoomIndex];
                RoomData chosenNewRoomData = chosenNewRoom.GetComponent<RoomData>();

                if (!chosenNewRoomData.GetConnectorPosition(chosenNewRoomConnector, out Vector3 connectorPositionNewRoom))
                    throw new Exception("Connector dont have position");
                
                Vector3 leftUpPosition = currentFreeConnectorPosition - connectorPositionNewRoom + chosenNewRoomData.LeftSize;
                Vector3 rightDownPosition = currentFreeConnectorPosition - connectorPositionNewRoom + chosenNewRoomData.RightSize;
            
                if(CheckIntersection(leftUpPosition, rightDownPosition)) continue;

                GameObject newRoom = Instantiate(possibleRooms[possibleRoomIndex], roomsParent);
                RoomData newRoomDataSpawned = newRoom.GetComponent<RoomData>();
                newRoom.transform.position = currentFreeConnectorPosition -  connectorPositionNewRoom;
                chosenRoomWithFreeConnectors.TakeConnector(connector);
                newRoomDataSpawned.TakeConnector(chosenNewRoomConnector);
                _enemiesPositions.AddRange(newRoomDataSpawned.GetFreeEnemiesPositions());
                _lootBoxesPositions.AddRange(newRoomDataSpawned.GetFreeLootBoxesPositions());

                _spawnedRooms.Add(newRoomDataSpawned);
            }
            
            if (chosenRoomWithFreeConnectors.GetFreeConnectors().Count <= 0)
            {
                _roomsWithFreeConnectors.Remove(chosenRoomWithFreeConnectors);
            }
        }
    }
    
    private bool CheckIntersection(Vector3 leftUpPosition, Vector3 rightDownPosition)
    {
        foreach (var spawnedRoom in _spawnedRooms)
        {

            if (leftUpPosition.x >= spawnedRoom.LeftSize.x && leftUpPosition.y <= spawnedRoom.LeftSize.y && 
                leftUpPosition.x < spawnedRoom.RightSize.x && leftUpPosition.y > spawnedRoom.RightSize.y)
                return true;
            if (rightDownPosition.x > spawnedRoom.LeftSize.x && rightDownPosition.y < spawnedRoom.LeftSize.y  && 
                rightDownPosition.x <= spawnedRoom.RightSize.x && rightDownPosition.y >= spawnedRoom.RightSize.y)
                return true;

            
            if (spawnedRoom.LeftSize.x >= leftUpPosition.x && spawnedRoom.LeftSize.y <= leftUpPosition.y  && 
                spawnedRoom.LeftSize.x < rightDownPosition.x && spawnedRoom.LeftSize.y > rightDownPosition.y)
                return true;
            if (spawnedRoom.RightSize.x > leftUpPosition.x && spawnedRoom.RightSize.y < leftUpPosition.y  && 
                spawnedRoom.RightSize.x <= rightDownPosition.x && spawnedRoom.RightSize.y >= rightDownPosition.y)
                return true;


            
            Vector3 leftDownPosition = leftUpPosition;
            leftDownPosition.y = rightDownPosition.y;
            Vector3 rightUpPosition = rightDownPosition;
            rightUpPosition.y = leftUpPosition.y;
            
            if (leftDownPosition.x > spawnedRoom.LeftSize.x && leftDownPosition.y < spawnedRoom.LeftSize.y  && 
                leftDownPosition.x < spawnedRoom.RightSize.x && leftDownPosition.y > spawnedRoom.RightSize.y)
                return true;
            if (rightUpPosition.x > spawnedRoom.LeftSize.x && rightUpPosition.y < spawnedRoom.LeftSize.y  && 
                rightUpPosition.x < spawnedRoom.RightSize.x && rightUpPosition.y > spawnedRoom.RightSize.y)
                return true;

            
            Vector3 spawnedRoomLeftDownPosition = spawnedRoom.LeftSize;
            spawnedRoomLeftDownPosition.y = spawnedRoom.RightSize.y;
            Vector3 spawnedRoomRightUpPosition = spawnedRoom.RightSize;
            spawnedRoomRightUpPosition.y = spawnedRoom.LeftSize.y;
            
            if (spawnedRoomLeftDownPosition.x >= leftUpPosition.x && spawnedRoomLeftDownPosition.y <= leftUpPosition.y  && 
                spawnedRoomLeftDownPosition.x < rightDownPosition.x && spawnedRoomLeftDownPosition.y > rightDownPosition.y)
                return true;
            if (spawnedRoomRightUpPosition.x > leftUpPosition.x && spawnedRoomRightUpPosition.y < leftUpPosition.y  && 
                spawnedRoomRightUpPosition.x <= rightDownPosition.x && spawnedRoomRightUpPosition.y >= rightDownPosition.y)
                return true;
        }

        return false;
    }

    #endregion

    
    private void GenerateEnemies()
    {
        int randomPercent = Random.Range(minPercentEnemies, maxPercentEnemies+1);

        float scale = (float)randomPercent / 100;
        int enemiesCount = (int)(_enemiesPositions.Count * scale);

        for (int i = 0; i < enemiesCount; i++)
        {
            int randomEnemyPositionIndex = Random.Range(0, _enemiesPositions.Count);
            int randomEnemyPrefabIndex = Random.Range(0, enemiesPrefabs.Count);

            GameObject newEnemy = Instantiate(enemiesPrefabs[randomEnemyPrefabIndex], enemiesParent);
            newEnemy.transform.position = _enemiesPositions[randomEnemyPositionIndex];
            
            _spawnedEnemies.Add(newEnemy);
            _enemiesPositions.Remove(_enemiesPositions[randomEnemyPositionIndex]);
        }
    }
    
    private void GenerateLootBoxes()
    {
        int randomPercent = Random.Range(minPercentLootBoxes, maxPercentLootBoxes+1);

        float scale = (float)randomPercent / 100;
        int lootBoxesCount = (int)(_lootBoxesPositions.Count * scale);

        for (int i = 0; i < lootBoxesCount; i++)
        {
            int randomLootBoxPositionIndex = Random.Range(0, _lootBoxesPositions.Count);
            int randomLootBoxPrefabIndex = Random.Range(0, lootBoxesPrefabs.Count);

            GameObject newLootBox = Instantiate(lootBoxesPrefabs[randomLootBoxPrefabIndex], lootBoxesParent);
            newLootBox.transform.position = _lootBoxesPositions[randomLootBoxPositionIndex];
            _spawnedLootBoxes.Add(newLootBox);
            _lootBoxesPositions.Remove(_lootBoxesPositions[randomLootBoxPositionIndex]);
        }
    }
}