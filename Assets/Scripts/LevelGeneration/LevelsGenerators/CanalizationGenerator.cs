using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LevelGeneration.LevelsGenerators
{
    public class CanalizationGenerator : LevelGeneratorBase
    {
        [Space]
        [SerializeField] [Range(0, 100)] private int minBlockCount;
        [SerializeField] [Range(0, 100)] private int maxBlockCount;
        [Space]
        [SerializeField] [Range(0, 100)] private int minForkCount;
        [SerializeField] [Range(0, 100)] private int maxForkCount;
        [Space]
        [SerializeField] private List<GameObject> mainBlocks;
        [Space]
        [SerializeField] private List<GameObject> forkBlocks;
        [Space]
        [SerializeField] private List<GameObject> hallwayBlocks;
        [Space]
        [SerializeField] private List<GameObject> endBlocks;
        
        private readonly Dictionary<ConnectorID, List<GameObject>> _mainBlocksPrefabs = new();
        private readonly Dictionary<ConnectorID, List<GameObject>> _forkBlocksPrefabs = new();
        private readonly Dictionary<ConnectorID, List<GameObject>> _endBlocksPrefabs = new();

        private int _iteration;
        
        protected override void OnUpdate()
        {
            if (test && Input.GetKeyDown(KeyCode.Q))
            {
                ClearLevel();
                GenerateLevel();
            }
        }

        protected override void Init()
        {
            base.Init();
        
            foreach (var connectorID in Enum.GetValues(typeof(ConnectorID)).Cast<ConnectorID>())
            {
                _mainBlocksPrefabs.Add(connectorID, new List<GameObject>());
                _endBlocksPrefabs.Add(connectorID, new List<GameObject>());
                _forkBlocksPrefabs.Add(connectorID, new List<GameObject>());
            }
        
            InitBlocksPrefabs(_mainBlocksPrefabs, new List<List<GameObject>>(){mainBlocks, hallwayBlocks});
            InitBlocksPrefabs(_endBlocksPrefabs, new List<List<GameObject>>(){endBlocks});
            InitBlocksPrefabs(_forkBlocksPrefabs, new List<List<GameObject>>(){forkBlocks});
        }

        protected override void ClearLevel()
        {
            if(GlobalLampBlinks.Instance)
                GlobalLampBlinks.Instance.Clear();
        
            base.ClearLevel();
        }
    
        protected override void GenerateBlocks()
        {
            _iteration++;
            if (_iteration > 100)
                throw new Exception("Too much iterations");
            
            CreateStartBlock();

            if (!CreateMainBlocks())
            {
                Debug.LogError("Main block cant be spawned");

                ClearLevel();
                GenerateBlocks();
                return;
            }

            if (!TryCreateFinalBlock())
            {
                Debug.LogError("Final block cant be spawned");

                ClearLevel();
                GenerateBlocks();
                return;
            }

            if (!TryCreateEndBlocks(_endBlocksPrefabs))
            {
                Debug.LogError("No end block cant be spawned");

                ClearLevel();
                GenerateBlocks();
                return;
            }
        }
    
        /// <summary>
        /// <para> Try spawn main rooms and return true if it possible. Else return false</para>
        /// </summary>
        /// <returns></returns>
        private bool CreateMainBlocks()
        {
            int blocksCount = Random.Range(minBlockCount, maxBlockCount + 1);
            
            int forksCount = Random.Range(minForkCount, maxForkCount + 1);

            int lastPossibleForkIndex = (int)((float)(blocksCount + 1) * 0.75f);
            List<int> forksIndexes = new List<int>();
            for (int i = 0; i < forksCount; i++)
            {
                int index = Random.Range(0, lastPossibleForkIndex);
            
                if(!forksIndexes.Contains(index)) forksIndexes.Add(index);
                else i--;
            }
        
            int blockIndex = 0;
            while (AllSpawnedBlocks.Count < blocksCount)
            {
                Dictionary<ConnectorID, List<GameObject>> prefabs =
                    forksIndexes.Contains(blockIndex) ? _forkBlocksPrefabs : _mainBlocksPrefabs;

                if (!TrySpawnMainBlock(AllBlocksWithFreeConnectors, prefabs)) return false;

                CheckConnectorsCollisions();
                blockIndex++;
            }

            return true;
        }
    }
}