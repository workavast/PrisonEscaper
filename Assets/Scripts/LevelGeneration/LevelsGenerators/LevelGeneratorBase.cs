using System;
using System.Collections.Generic;
using System.Linq;
using Character;
using Enemies;
using SerializableDictionaryExtension;
using UnityEngine;
using Zenject;
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

        [Inject] private readonly EnemiesFactory _enemiesFactory;
        [Inject] private DiContainer _container;

        protected Vector3 PlayerSpawnPosition;
        protected GameObject Player;
        protected readonly List<IBlockData> AllSpawnedBlocks = new();
        protected readonly List<BlockDataPrototype> AllBlocksPrototypes = new();
        protected readonly List<BlockDataPrototype> AllBlocksPrototypesWithFreeConnectors = new();
        
        private readonly Dictionary<ConnectorID, List<GameObject>> _finalBlocksPrefabs = new();
        private bool _finalBlockSpawned;

        public event Action<Player> OnPlayerSpawned;
        public event Action OnGenereationStart;
        public event Action OnGenerateFinished; 
        
        private void Awake() => OnAwake();
        private void Start() => OnStart();
        private void Update() => OnUpdate();

        protected virtual void OnAwake()
            => Init();

        protected virtual void OnStart()
            => GenerateLevel();
        
        protected virtual void OnUpdate(){}
    
        protected virtual void Init()
        {
            var player = FindObjectOfType<Player>();
            if(player != null) Player = player.gameObject;
            
            enemiesData.Init();
            lootBoxesData.Init();
            trapsData.Init();
        
            foreach (var connectorID in Enum.GetValues(typeof(ConnectorID)).Cast<ConnectorID>())
            {
                _finalBlocksPrefabs.Add(connectorID, new List<GameObject>());
            }
        
            InitBlocksPrefabs(_finalBlocksPrefabs, new List<List<GameObject>>(){finalBlocks});
        }

        protected void InitBlocksPrefabs(IReadOnlyDictionary<ConnectorID, List<GameObject>> prefabs, 
            IReadOnlyList<List<GameObject>> prefabsSources)
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
            OnGenereationStart?.Invoke();
            
            GenerateBlocks();
            
            SpawnPlayer();
            SpawnEnemies(enemiesData);
            GenerateSpawnableObjects(lootBoxesData);
            GenerateSpawnableObjects(trapsData);
            
            OnGenerateFinished?.Invoke();
        }

        private void SpawnPlayer()
        {
            if (Player is null)
            {
                Player = _container.InstantiatePrefab(playerPrefab, PlayerSpawnPosition, Quaternion.Euler(0,0,0), null);
                OnPlayerSpawned?.Invoke(Player.GetComponent<Player>());
            }
            else
                Player.transform.position = PlayerSpawnPosition;
        }
        
        protected virtual void ClearLevel()
        {
            foreach (var blocks in AllSpawnedBlocks)
                Destroy(blocks.gameObject);
            
            AllSpawnedBlocks.Clear();
            AllBlocksPrototypes.Clear();
            AllBlocksPrototypesWithFreeConnectors.Clear();
        
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
            if (!_container.InstantiatePrefab(startBlock, blocksParent).TryGetComponent(out IStartBlockData startBlockData))
                throw new Exception("Start block dont have script startBlockData");
        
            InitBlockData(startBlockData);
            var blockPrototype = startBlockData.GetPreparingBlock();
            blockPrototype.SetPosition(Vector3.zero);
            
            PlayerSpawnPosition = startBlockData.PlayerSpawnPosition;
            AllSpawnedBlocks.Add(startBlockData);

            AllBlocksPrototypes.Add(blockPrototype);
            AllBlocksPrototypesWithFreeConnectors.Add(blockPrototype);
        }
    
        /// <sumary>
        /// <para> Return true if final block spawned, if final block cant be spawned return false</para>
        /// </sumary>
        protected bool TryCreateFinalBlock()
        {
            if (_finalBlockSpawned)
                throw new Exception("Final block has already been spawned");
        
            var connectorsDistances = new List<(float, BlockDataPrototype, ConnectorID)>();
            foreach (var block in AllBlocksPrototypesWithFreeConnectors)//calculate and sort distances from start block to all free connectors
            {
                IReadOnlyDictionary<ConnectorID, Vector3>  connectorPositions = block.FreeConnectorsPositions;
                foreach (var connector in connectorPositions)
                {
                    float newDistance = Mathf.Abs(Vector3.Distance(startBlock.transform.position, connector.Value));
                
                    var newConnectors = new List<(float, BlockDataPrototype, ConnectorID)>();
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
                var blockForConnection = connectorDistance.Item2;
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
            int iterationsCount = AllBlocksPrototypesWithFreeConnectors.Count;
            
            for (int i = 0; i < iterationsCount; i++)
            {
                var blockForConnection = AllBlocksPrototypesWithFreeConnectors.First();

                IReadOnlyList<ConnectorID> freeConnectors = blockForConnection.FreeConnectors;
                foreach (var freeConnectorID in freeConnectors)
                    if (!TrySpawnBlock(blockForConnection, freeConnectorID, blocksPrefabs))
                        return false;
            }

            return true;
        }
    
        protected bool TrySpawnMainBlock(IEnumerable<BlockDataPrototype> blocksWithFreeConnectors, 
            IReadOnlyDictionary<ConnectorID, List<GameObject>> blocksPrefabs)
        {
            var possibleBlocks = new List<BlockDataPrototype>(blocksWithFreeConnectors);
            for (int i = 0; i < possibleBlocks.Count; i++)
            {
                int blockPrefabIndex = Random.Range(0, possibleBlocks.Count);
                var blockForConnection = possibleBlocks[blockPrefabIndex];
            
                var freeConnectorsIDs = new List<ConnectorID>(blockForConnection.FreeConnectors);
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
        private bool TrySpawnBlock(BlockDataPrototype blockForConnection, ConnectorID freeConnectorID, 
            IReadOnlyDictionary<ConnectorID, List<GameObject>> blocksPrefabs)
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
                    SpawnBlock(newBlockPrefab, newBlockPosition, blockForConnection, 
                        freeConnectorID, newBlockConnectorID);
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
        
            foreach (var spawnedBlock in AllBlocksPrototypes)
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

        private void SpawnBlock(GameObject newBlockPrefab, Vector3 newBlockPosition, BlockDataPrototype blockForConnection, 
            ConnectorID existBlockConnector, ConnectorID newBlockConnectorID)
        {
            var newBlock = newBlockPrefab.GetComponent<IBlockData>().GetPreparingBlock();
            newBlock.SetPosition(newBlockPosition);
            
            blockForConnection.OccupyConnector(existBlockConnector);
            newBlock.OccupyConnector(newBlockConnectorID);
            AllBlocksPrototypes.Add(newBlock);
            
            if (newBlock.FreeConnectors.Count > 0)
                AllBlocksPrototypesWithFreeConnectors.Add(newBlock);
        
            if (blockForConnection.FreeConnectors.Count <= 0)
                AllBlocksPrototypesWithFreeConnectors.Remove(blockForConnection);
        }

        private void InitBlockData(IBlockData blockData)
        {
            enemiesData.spawnPoints.AddRange(blockData.EnemiesPoints);
            lootBoxesData.spawnPoints.AddRange(blockData.LootBoxesPoints);
            trapsData.spawnPoints.AddRange(blockData.TrapsPoints);
        }
    
        protected void CheckConnectorsCollisions()
        {
            for (int i = 0; i < AllBlocksPrototypesWithFreeConnectors.Count; i++)
            {
                IReadOnlyDictionary<ConnectorID, Vector3> mainBlockConnectors = AllBlocksPrototypesWithFreeConnectors[i].FreeConnectorsPositions;
                for (int n = i+1; n < AllBlocksPrototypesWithFreeConnectors.Count; n++)
                {
                    IReadOnlyDictionary<ConnectorID, Vector3>  curBlockConnectors = AllBlocksPrototypesWithFreeConnectors[n].FreeConnectorsPositions;

                    foreach (var mainBlockConnector in mainBlockConnectors)
                    {
                        foreach (var curBlockConnector in curBlockConnectors)
                        {
                            if (!(Mathf.Abs(Vector3.Distance(mainBlockConnector.Value, curBlockConnector.Value)) < 0.1f)) 
                                continue;
                        
                            AllBlocksPrototypesWithFreeConnectors[i].OccupyConnector(mainBlockConnector.Key);
                            AllBlocksPrototypesWithFreeConnectors[n].OccupyConnector(curBlockConnector.Key);
                        }
                    }
                        
                    if (AllBlocksPrototypesWithFreeConnectors[i].FreeConnectors.Count <= 0)
                    {
                        AllBlocksPrototypesWithFreeConnectors.RemoveAt(i);
                        i--;
                        n--;
                    }   
                                                                        
                    if (AllBlocksPrototypesWithFreeConnectors[n].FreeConnectors.Count <= 0)
                    {
                        AllBlocksPrototypesWithFreeConnectors.RemoveAt(n);
                        n--;
                    } 
                }
            }
        }
        
        protected void ApplyGeneration()
        {
            foreach (var blockPrototype in AllBlocksPrototypes)
            {
                var block = _container.InstantiatePrefab(blockPrototype.Prefab, blockPrototype.Position,
                    Quaternion.identity, blocksParent).GetComponent<IBlockData>();
                
                AllSpawnedBlocks.Add(block);
                InitBlockData(block);
            }
        }
        #endregion
    
    
        #region GenerateSpawnableObjects
        private void SpawnEnemies(SpawnableObjectData<EnemyID> data)
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
                SpawnableObjectCell<EnemyID> pair = data.spawnPoints[pairIndex];

                int idIndex = Random.Range(0, pair.IDs.Count);
                EnemyID id = pair.IDs[idIndex];

                if (!data.Prefabs.ContainsKey(id))
                {
                    Debug.LogWarning($"Un exist key: {id} in the {data} in the {data.Prefabs}");
                    i--;
                    continue;
                }

                var enemy = _enemiesFactory.Create(id, pair.Transform.position);
                data.spawned.Add(enemy.gameObject);
            
                data.spawnPoints.RemoveAt(pairIndex);
            }
        }
        
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
            
                GameObject newObject = _container.InstantiatePrefab(data.Prefabs[id], pair.Transform.position, Quaternion.identity, data.Parent);
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
            [field:SerializeField] public SerializableDictionary<TIdentifier, GameObject> Prefabs { get; private set; }
            
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