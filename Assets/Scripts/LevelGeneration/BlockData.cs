using System;
using System.Collections.Generic;
using System.Linq;
using Enemies;
using LevelGeneration.Marks;
using UnityEngine;

namespace LevelGeneration
{
    public class BlockData : MonoBehaviour, IBlockData, IBlockDataForGUI
    {
        [SerializeField] private BlockType blockType;

        [SerializeField] private Transform leftUpSizePoint;
        [SerializeField] private Transform rightDownSizePoint;
        [SerializeField] private DictionaryInspector<ConnectorID, Transform> connectorsData;

        [SerializeField] private List<SpawnableObjectCell<EnemyID>> enemyPoints;
        [SerializeField] private List<SpawnableObjectCell<LootBoxID>> lootBoxPoints;
        [SerializeField] private List<SpawnableObjectCell<TrapID>> trapsPoints;
        
        public Vector3 LeftUpSizePoint => leftUpSizePoint.position;
        public Vector3 RightDownSizePoint => rightDownSizePoint.position;
        
        public IReadOnlyList<ConnectorID> AllConnectors => connectorsData
            .Select(data => data.Key).ToList();
        public IReadOnlyDictionary<ConnectorID, Vector3> ConnectorsPositions => connectorsData
            .ToDictionary(connector => connector.Key, connector => connectorsData[connector.Key].position);

        public IReadOnlyList<SpawnableObjectCell<EnemyID>> EnemiesPoints => enemyPoints;
        public IReadOnlyList<SpawnableObjectCell<LootBoxID>> LootBoxesPoints => lootBoxPoints;
        public IReadOnlyList<SpawnableObjectCell<TrapID>> TrapsPoints => trapsPoints;
        
        private void Awake()
        {
#if UNITY_EDITOR
            CheckDuplicates(trapsPoints);
            CheckDuplicates(enemyPoints);
#endif
        }

#if UNITY_EDITOR
        private void CheckDuplicates<TId>(List<SpawnableObjectCell<TId>> points)
            where TId : Enum
        {
            var ids = new List<TId>();
            foreach (var trapPoint in points)
            {
                ids.Clear();
                foreach (var id in trapPoint.IDs)
                {
                    if (ids.Contains(id))
                        throw new Exception($"Duplicate of {id} in {gameObject.name}");
                    ids.Add(id);
                }
            }
        }
#endif
        
        public Vector3 GetConnectorPosition(ConnectorID id)
        {
            if (connectorsData.ContainsKey(id))
                return connectorsData[id].position;
            
            throw new Exception($"Invalid ConnectorID: {id} in {this}");
        }
        
        public BlockDataPrototype GetPreparingBlock() 
            => new(blockType, gameObject, leftUpSizePoint, rightDownSizePoint, connectorsData);

        #region IBlockDataForGUI
        
        public void SetAllPoints()
        {
            SetEnemiesPoints();
            SetLootBoxPoints();
            SetTrapsPoints();
        }

        public void SetEnemiesPoints() => SetSpawnableObjectsPoints<EnemyID, EnemyPointMark>(ref enemyPoints);
        public void SetLootBoxPoints() => SetSpawnableObjectsPoints<LootBoxID, LootBoxPointMark>(ref lootBoxPoints);
        public void SetTrapsPoints() => SetSpawnableObjectsPoints<TrapID, TrapPointMark>(ref trapsPoints);

        public void ReSetAllPoints()
        {
            ReSetEnemiesPoints();
            ReSetLootBoxPoints();
            ReSetTrapsPoints();
        }

        public void ReSetEnemiesPoints() => ReSetSpawnableObjectsPoints(ref enemyPoints, SetEnemiesPoints);
        public void ReSetLootBoxPoints() => ReSetSpawnableObjectsPoints(ref lootBoxPoints, SetLootBoxPoints);
        public void ReSetTrapsPoints() => ReSetSpawnableObjectsPoints(ref trapsPoints, SetTrapsPoints);

        public void SetSizePoints()
        {
            leftUpSizePoint = GetComponentInChildren<LeftUpSizePointMarker>().transform;
            rightDownSizePoint = GetComponentInChildren<RightDownSizePointMarker>().transform;
        }

        private void SetSpawnableObjectsPoints<TIdentifier, TSpawnableObjectMark>(ref List<SpawnableObjectCell<TIdentifier>> spawnableObjectsCells)
            where TIdentifier : Enum
            where TSpawnableObjectMark : SpawnableObjectMarkBase
        {
            TSpawnableObjectMark[] marks = GetComponentsInChildren<TSpawnableObjectMark>();

            if (marks.Length <= 0) return;

            List<Transform> marksTransforms = new();
            marksTransforms.AddRange(marks.Select(mark => mark.transform));

            for (int i = 0; i < marksTransforms.Count; i++)
            {
                if (spawnableObjectsCells.Any(spawnableObjectsCell =>
                        spawnableObjectsCell.Transform == marksTransforms[i]))
                {
                    marksTransforms.RemoveAt(i);
                    i--;
                }
            }

            TIdentifier[] ids = Enum.GetValues(typeof(TIdentifier)).Cast<TIdentifier>().ToArray();
            spawnableObjectsCells.AddRange(marksTransforms.Select(markTransform =>
                new SpawnableObjectCell<TIdentifier>(markTransform, ids)));

            for (int i = 0; i < spawnableObjectsCells.Count; i++)
                if (!spawnableObjectsCells[i].Transform)
                {
                    spawnableObjectsCells.RemoveAt(i);
                    i--;
                }
        }

        private void ReSetSpawnableObjectsPoints<TIdentifier>(ref List<SpawnableObjectCell<TIdentifier>> spawnableObjectsCells, Action setMethod)
            where TIdentifier : Enum
        {
            spawnableObjectsCells = new List<SpawnableObjectCell<TIdentifier>>();
            setMethod();
        }

        #endregion

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (leftUpSizePoint == null || rightDownSizePoint == null) return;

            switch (blockType)
            {
                case BlockType.Main: Gizmos.color = Color.white; break;
                case BlockType.Fork: Gizmos.color = Color.red; break;
                case BlockType.Start: Gizmos.color = Color.green; break;
                case BlockType.Final: Gizmos.color = Color.blue; break;
                default: throw new Exception("Un expected block type");
            }

            Vector3 center = (leftUpSizePoint.position + rightDownSizePoint.position) / 2;
            float with = (leftUpSizePoint.position.x - center.x) * 2;
            float height = (leftUpSizePoint.position.y - center.y) * 2;
            Gizmos.DrawWireCube(center, new Vector3(with, height, 0));
        }
#endif
    }
}