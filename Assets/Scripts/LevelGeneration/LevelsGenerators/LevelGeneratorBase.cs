using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LevelGeneration.LevelsGenerators
{
    public abstract class LevelGeneratorBase : MonoBehaviour
    {
        [SerializeField] protected bool test;
        [Space]
        [SerializeField] protected GameObject playerPrefab;
        [Space]
        [SerializeField] private Transform blocksParent;
        [Space]
        [SerializeField] private GameObject startBlock;
        [SerializeField] private List<GameObject> finalBlocks;
        [Space]
        [SerializeField] private SpawnableObjectData<EnemyID> enemiesData;
        [SerializeField] private SpawnableObjectData<LootBoxID> lootBoxesData;
        [SerializeField] private SpawnableObjectData<TrapID> trapsData;
        [Space]

        protected Vector3 PlayerSpawnPosition;
        protected GameObject Player;
    
        private readonly Dictionary<ConnectorID, List<GameObject>> _finalBlocksPrefabs = new();

        protected List<BlockData> AllBlocksWithFreeConnectors = new();
        protected List<IBlockData> AllSpawnedBlocks = new();
    
        private bool _finalBlockSpawned;

        private void Awake() => OnAwake();
        private void Start() => OnStart();
        private void Update() => OnUpdate();

        protected virtual void OnAwake(){
            Init();
            GenerateLevel();
        }
        protected virtual void OnStart(){}
        protected virtual void OnUpdate(){}
    
        protected virtual void Init()
        {
            enemiesData.Init();
            lootBoxesData.Init();
            trapsData.Init();
        
            foreach (var connectorID in Enum.GetValues(typeof(ConnectorID)).Cast<ConnectorID>())
            {
                _finalBlocksPrefabs.Add(connectorID, new List<GameObject>());
            }
        
            InitBlocksPrefabs(_finalBlocksPrefabs, new List<List<GameObject>>(){finalBlocks});
        }

        protected void InitBlocksPrefabs(IReadOnlyDictionary<ConnectorID, List<GameObject>> prefabs, IReadOnlyList<List<GameObject>> prefabsSources)
        {
            List<GameObject> allPrefabs = new List<GameObject>();
            foreach (var source in prefabsSources)
                allPrefabs.AddRange(source);

            foreach (var prefab in allPrefabs)
            {
                IReadOnlyList<ConnectorID> connectorIDs = prefab.GetComponent<IBlockData>().AllConnectors;
                foreach (var connectorID in connectorIDs)
                    prefabs[connectorID].Add(prefab);
            }
        }
    
        protected void GenerateLevel()
        {
            GenerateBlocks();
            GenerateSpawnableObjects(enemiesData);
            GenerateSpawnableObjects(lootBoxesData);
            GenerateSpawnableObjects(trapsData);
        }
    
        protected virtual void ClearLevel()
        {
            foreach (var blocks in AllSpawnedBlocks)
                Destroy(blocks.gameObject);
            
            foreach (var blocks in AllBlocksWithFreeConnectors)
                Destroy(blocks.gameObject);
        
            AllSpawnedBlocks = new List<IBlockData>();
            AllBlocksWithFreeConnectors = new List<BlockData>();
        
            _finalBlockSpawned = false;
        
            enemiesData.DestroySpawnedObjects();
            lootBoxesData.DestroySpawnedObjects();
            trapsData.DestroySpawnedObjects();
        
            enemiesData.Init();
            lootBoxesData.Init();
            trapsData.Init();
        }

    
        #region GenerateBlocks

        protected abstract void GenerateBlocks();
    
        protected void CreateStartBlock()
        {
            if (!Instantiate(startBlock, blocksParent).TryGetComponent(out IStartBlockData startBlockData))
                throw new Exception("Start block dont have script startBlockData");
        
            InitBlockData(startBlockData);
            PlayerSpawnPosition = startBlockData.PlayerSpawnPosition;
            AllSpawnedBlocks.Add(startBlockData);
            AllBlocksWithFreeConnectors.Add(startBlockData as BlockData);
        }
    
        /// <sumary>
        /// <para> Return true if final block spawned, if final block cant be spawned return false</para>
        /// </sumary>
        protected bool TryCreateFinalBlock()
        {
            if (_finalBlockSpawned)
                throw new Exception("Final block has already been spawned");
        
            var connectorsDistances = new List<(float, IBlockData, ConnectorID)>();
            foreach (var block in AllBlocksWithFreeConnectors)//calculate and sort distances from start block to all free connectors
            {
                IReadOnlyDictionary<ConnectorID, Vector3>  connectorPositions = block.FreeConnectorsPositions;
                foreach (var connector in connectorPositions)
                {
                    float newDistance = Mathf.Abs(Vector3.Distance(startBlock.transform.position, connector.Value));
                
                    var newConnectors = new List<(float, IBlockData, ConnectorID)>();
                    foreach (var connectorDistance in connectorsDistances)
                    {
                        if (connectorDistance.Item1 >= newDistance)
                        {
                            newConnectors.Add(connectorDistance);
                        }
                        else
                        {
                            newConnectors.Add((newDistance, block, connector.Key));
                            newConnectors.Add(connectorDistance);
                            newDistance = 0;
                        }
                    }
                
                    if (newDistance != 0)
                        newConnectors.Add((newDistance, block, connector.Key));
        
                    connectorsDistances = newConnectors;
                }
            }
        
            foreach (var connectorDistance in connectorsDistances)
            {
                IBlockData blockForConnection = connectorDistance.Item2;
                ConnectorID freeConnectorID = connectorDistance.Item3;

                if (TrySpawnBlock(blockForConnection, freeConnectorID, _finalBlocksPrefabs))
                {
                    _finalBlockSpawned = true;
                    return true;    
                }
            }
    
            return false;
        }
    
        /// <sumary>
        /// <para> Return true if all ends spawned, if no block that can be spawned return false</para>
        /// </sumary>
        protected bool TryCreateEndBlocks(IReadOnlyDictionary<ConnectorID, List<GameObject>> blocksPrefabs)
        {
            int iterationsCount = AllBlocksWithFreeConnectors.Count;
            for (int i = 0; i < iterationsCount; i++)
            {
                IBlockData blockForConnection = AllBlocksWithFreeConnectors.First();

                IReadOnlyList<ConnectorID> freeConnectors = blockForConnection.FreeConnectors;
                foreach (var freeConnectorID in freeConnectors)
                    if (!TrySpawnBlock(blockForConnection, freeConnectorID, blocksPrefabs))
                        return false;
            }

            return true;
        }
    
        protected bool TrySpawnMainBlock(List<BlockData> blocksWithFreeConnectors, IReadOnlyDictionary<ConnectorID, List<GameObject>> blocksPrefabs)
        {
            List<BlockData> possibleBlocks = new List<BlockData>(blocksWithFreeConnectors);
            for (int i = 0; i < possibleBlocks.Count; i++)
            {
                int blockPrefabIndex = Random.Range(0, possibleBlocks.Count);
                IBlockData blockForConnection = possibleBlocks[blockPrefabIndex];
            
                List<ConnectorID> freeConnectorsIDs = new List<ConnectorID>(blockForConnection.FreeConnectors);

                for (int j = 0; j < freeConnectorsIDs.Count; j++)
                {
                    int prefabBlockConnectorIndex = Random.Range(0, freeConnectorsIDs.Count);
                    ConnectorID freeConnectorID = freeConnectorsIDs[prefabBlockConnectorIndex];
                
                    if (TrySpawnBlock(blockForConnection, freeConnectorID, blocksPrefabs))
                        return true;
                
                    freeConnectorsIDs.RemoveAt(prefabBlockConnectorIndex);
                    j--;
                }
            
                possibleBlocks.RemoveAt(blockPrefabIndex);
                i--;
            }
        
            return false;
        }
    
        /// <summary>
        ///  <para> Spawn some prefab from blocksPrefabs that can be spawned, and return true. If no block that can be spawned return false</para>
        /// </summary>
        private bool TrySpawnBlock(IBlockData blockForConnection, ConnectorID freeConnectorID, IReadOnlyDictionary<ConnectorID, List<GameObject>> blocksPrefabs)
        {
            ConnectorID newBlockConnectorID = CalculateConstConnector(freeConnectorID);
        
            List<GameObject> possiblePrefabs = new List<GameObject>(blocksPrefabs[newBlockConnectorID]);
            for (int i = 0; i < possiblePrefabs.Count; i++)
            {
                GameObject newBlockPrefab = FindRandomBlockPrefabForConnection(possiblePrefabs, out int chosenIndex);
                IBlockData newBlockPrefabData = newBlockPrefab.GetComponent<IBlockData>();
            
                Vector3 freeConnectorPosition = blockForConnection.GetConnectorPosition(freeConnectorID);
                Vector3 newBlockConnectorPosition = newBlockPrefabData.GetConnectorPosition(newBlockConnectorID);
                Vector3 newBlockPosition = freeConnectorPosition - newBlockConnectorPosition;

                if (CheckIntersections(newBlockPrefabData, newBlockPosition))
                {
                    possiblePrefabs.RemoveAt(chosenIndex);
                    i--;
                }
                else
                {
                    SpawnBlock(newBlockPrefab, newBlockPosition, blockForConnection, freeConnectorID, newBlockConnectorID);
                    return true;
                }
            }

            return false;
        }

        private ConnectorID CalculateConstConnector(ConnectorID connectorID)
        {
            switch (connectorID)
            {
                case ConnectorID.Left: return ConnectorID.Right;
                case ConnectorID.Right: return ConnectorID.Left;
                case ConnectorID.Up: return ConnectorID.Down;
                case ConnectorID.Down: return ConnectorID.Up;
                default: throw new Exception("Un correct connectorId");
            }
        }
    
        private GameObject FindRandomBlockPrefabForConnection(IReadOnlyList<GameObject> blocksPrefabs, out int chosenIndex)
        {
            if (blocksPrefabs.Count == 0)
                throw new Exception("possibleBlocks.Count == 0");
        
            chosenIndex = Random.Range(0, blocksPrefabs.Count);
            return blocksPrefabs[chosenIndex];
        }
    
        /// <sumary>
        /// <para> Return true if intersection detected else return false</para>
        /// </sumary>
        private bool CheckIntersections(IBlockData newBlock, Vector3 newBlockPosition)
        {
            Vector3 leftSize = newBlock.LeftUpSizePoint;
            Vector3 rightSize = newBlock.RightDownSizePoint;

            Vector3 leftUpPosition = newBlockPosition + leftSize;
            Vector3 rightDownPosition = newBlockPosition + rightSize;
        
            foreach (var spawnedBlock in AllSpawnedBlocks)
            {
                //if leftUpPosition of new block inside spawned block
                if (spawnedBlock.LeftUpSizePoint.x <= leftUpPosition.x && leftUpPosition.x < spawnedBlock.RightDownSizePoint.x &&
                    spawnedBlock.RightDownSizePoint.y < leftUpPosition.y && leftUpPosition.y <= spawnedBlock.LeftUpSizePoint.y)
                    return true;
                //if rightDownPosition of new block inside spawned block
                if (spawnedBlock.LeftUpSizePoint.x < rightDownPosition.x && rightDownPosition.x <= spawnedBlock.RightDownSizePoint.x &&
                    spawnedBlock.RightDownSizePoint.y <= rightDownPosition.y && rightDownPosition.y < spawnedBlock.LeftUpSizePoint.y)
                    return true;

                //if LeftSize of spawned block inside new block
                if (leftUpPosition.x <= spawnedBlock.LeftUpSizePoint.x && spawnedBlock.LeftUpSizePoint.x < rightDownPosition.x &&
                    rightDownPosition.y < spawnedBlock.LeftUpSizePoint.y && spawnedBlock.LeftUpSizePoint.y <= leftUpPosition.y)
                    return true;
                //if RightSize of spawned block inside new block
                if (leftUpPosition.x < spawnedBlock.RightDownSizePoint.x && spawnedBlock.RightDownSizePoint.x <= rightDownPosition.x &&
                    rightDownPosition.y <= spawnedBlock.RightDownSizePoint.y && spawnedBlock.RightDownSizePoint.y < leftUpPosition.y)
                    return true;
            
                Vector3 leftDownPosition = leftUpPosition;
                leftDownPosition.y = rightDownPosition.y;
                Vector3 rightUpPosition = rightDownPosition;
                rightUpPosition.y = leftUpPosition.y;
                //if leftDownPosition of new block inside spawned block
                if (spawnedBlock.LeftUpSizePoint.x <= leftDownPosition.x && leftDownPosition.x < spawnedBlock.RightDownSizePoint.x &&
                    spawnedBlock.RightDownSizePoint.y <= leftDownPosition.y && leftDownPosition.y < spawnedBlock.LeftUpSizePoint.y)
                    return true;
                //if rightUpPosition of new block inside spawned block
                if (spawnedBlock.LeftUpSizePoint.x < rightUpPosition.x && rightUpPosition.x <= spawnedBlock.RightDownSizePoint.x &&
                    spawnedBlock.RightDownSizePoint.y < rightUpPosition.y && rightUpPosition.y <= spawnedBlock.LeftUpSizePoint.y)
                    return true;
            
                Vector3 spawnedBlockLeftDownPosition = spawnedBlock.LeftUpSizePoint;
                spawnedBlockLeftDownPosition.y = spawnedBlock.RightDownSizePoint.y;
                Vector3 spawnedBlockRightUpPosition = spawnedBlock.RightDownSizePoint;
                spawnedBlockRightUpPosition.y = spawnedBlock.LeftUpSizePoint.y;
                //if spawnedBlockLeftDownPosition of spawned block inside new block
                if (leftUpPosition.x <= spawnedBlockLeftDownPosition.x && spawnedBlockLeftDownPosition.x < rightDownPosition.x &&
                    rightDownPosition.y <= spawnedBlockLeftDownPosition.y && spawnedBlockLeftDownPosition.y < leftUpPosition.y)
                    return true;
                //if spawnedBlockRightUpPosition of spawned block inside new block
                if (leftUpPosition.x < spawnedBlockRightUpPosition.x && spawnedBlockRightUpPosition.x <= rightDownPosition.x &&
                    rightDownPosition.y < spawnedBlockRightUpPosition.y && spawnedBlockRightUpPosition.y <= leftUpPosition.y)
                    return true;


                //checking that the sides do not intersect
                if (spawnedBlock.RightDownSizePoint.y < leftUpPosition.y && leftUpPosition.y <= spawnedBlock.LeftUpSizePoint.y &&
                    leftUpPosition.x < spawnedBlock.LeftUpSizePoint.x && spawnedBlock.LeftUpSizePoint.x < rightUpPosition.x)
                    return true;
                if (spawnedBlock.LeftUpSizePoint.x <= leftUpPosition.x && leftUpPosition.x < spawnedBlock.RightDownSizePoint.x &&
                    rightDownPosition.y < spawnedBlock.LeftUpSizePoint.y && spawnedBlock.LeftUpSizePoint.y <= leftUpPosition.y)
                    return true;
            }
        
            return false;
        }

        private void SpawnBlock(GameObject newBlockPrefab, Vector3 newBlockPosition, IBlockData blockForConnection, ConnectorID existBlockConnector, ConnectorID newBlockConnectorID)
        {
            IBlockData newBlock = Instantiate(newBlockPrefab, newBlockPosition, Quaternion.identity, blocksParent).GetComponent<IBlockData>();
            InitBlockData(newBlock);
            blockForConnection.OccupyConnector(existBlockConnector);
            newBlock.OccupyConnector(newBlockConnectorID);
        
            AllSpawnedBlocks.Add(newBlock);
        
            if (newBlock.FreeConnectors.Count > 0)
                AllBlocksWithFreeConnectors.Add(newBlock as BlockData);
        
            if (blockForConnection.FreeConnectors.Count <= 0)
                AllBlocksWithFreeConnectors.Remove(blockForConnection as BlockData);
        }

        private void InitBlockData(IBlockData blockData)
        {
            enemiesData.spawnPoints.AddRange(blockData.EnemiesPoints);
            lootBoxesData.spawnPoints.AddRange(blockData.LootBoxesPoints);
            trapsData.spawnPoints.AddRange(blockData.TrapsPoints);
        }
    
        protected void CheckConnectorsCollisions()
        {
            for (int i = 0; i < AllBlocksWithFreeConnectors.Count; i++)
            {
                IReadOnlyDictionary<ConnectorID, Vector3> mainBlockConnectors = AllBlocksWithFreeConnectors[i].FreeConnectorsPositions;
                for (int n = i+1; n < AllBlocksWithFreeConnectors.Count; n++)
                {
                    IReadOnlyDictionary<ConnectorID, Vector3>  curBlockConnectors = AllBlocksWithFreeConnectors[n].FreeConnectorsPositions;

                    foreach (var mainBlockConnector in mainBlockConnectors)
                    {
                        foreach (var curBlockConnector in curBlockConnectors)
                        {
                            if (!(Mathf.Abs(Vector3.Distance(mainBlockConnector.Value, curBlockConnector.Value)) < 0.1f)) 
                                continue;
                        
                            AllBlocksWithFreeConnectors[i].OccupyConnector(mainBlockConnector.Key);
                            AllBlocksWithFreeConnectors[n].OccupyConnector(curBlockConnector.Key);
                        }
                    }
                        
                    if (AllBlocksWithFreeConnectors[i].FreeConnectors.Count <= 0)
                    {
                        AllBlocksWithFreeConnectors.RemoveAt(i);
                        i--;
                        n--;
                    }   
                                                                        
                    if (AllBlocksWithFreeConnectors[n].FreeConnectors.Count <= 0)
                    {
                        AllBlocksWithFreeConnectors.RemoveAt(n);
                        n--;
                    } 
                }
            }
        }
    
        #endregion
    
    
        #region GenerateSpawnableObjects
    
        private void GenerateSpawnableObjects<TIdentifier>(SpawnableObjectData<TIdentifier> data)
            where TIdentifier : Enum
        {
            int randomPercent = Random.Range(data.MinPercent, data.MaxPercent+1);
        
            float scale = (float)randomPercent / 100;
            int trapsCount = (int)(data.spawnPoints.Count * scale);

            int iter = 0;
            for (int i = 0; i < trapsCount; i++)
            {
                iter++;
                if (iter >= 500) throw new Exception("TO MUCH");

                int pairIndex = Random.Range(0, data.spawnPoints.Count);
                SpawnableObjectCell<TIdentifier> pair = data.spawnPoints[pairIndex];

                int idIndex = Random.Range(0, pair.IDs.Count);
                TIdentifier id = pair.IDs[idIndex];

                if (!data.Prefabs.ContainsKey(id))
                {
                    Debug.LogWarning($"Un exist key: {id} in the {data} in the {data.Prefabs}");
                    i--;
                    continue;
                }
            
                GameObject newObject = Instantiate(data.Prefabs[id], pair.Transform.position, Quaternion.identity, data.Parent);
                data.spawned.Add(newObject);
            
                data.spawnPoints.RemoveAt(pairIndex);
            }
        }
    
        [Serializable]
        private class SpawnableObjectData<TIdentifier>
            where TIdentifier : Enum
        {
            [field: SerializeField] public Transform Parent { get; private set; }
            [field:SerializeField] [field:Range(0, 100)] public int MinPercent { get; private set; }
            [field:SerializeField] [field:Range(0, 100)] public int MaxPercent { get; private set; }
            [field:SerializeField] public DictionaryInspector<TIdentifier, GameObject> Prefabs { get; private set; }
            [HideInInspector] public List<TIdentifier> IDs = new();
            [HideInInspector] public List<SpawnableObjectCell<TIdentifier>> spawnPoints = new();
            [HideInInspector] public List<GameObject> spawned = new();

            public void Init()
            {
                IDs = new List<TIdentifier>();
                foreach (var id in Enum.GetValues(typeof(TIdentifier)).Cast<TIdentifier>())
                    IDs.Add(id);

                spawnPoints = new List<SpawnableObjectCell<TIdentifier>>();
                spawned = new List<GameObject>();
            }

            public void DestroySpawnedObjects()
            {
                foreach (var spawnedObject in spawned)
                    Destroy(spawnedObject);
            }
        }
    
        #endregion
    }
}