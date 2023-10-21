using System;
using System.Collections.Generic;
using UnityEngine;

namespace LevelGeneration
{
    [Serializable]
    public class SpawnableObjectCell<TIdentifier>
        where TIdentifier : Enum
    {
        [SerializeField] private Transform transform;
        [SerializeField] private List<TIdentifier> ids;

        public Transform Transform => transform;
        public List<TIdentifier> IDs => ids;

        public SpawnableObjectCell(Transform newTransform)
        {
            transform = newTransform;
            ids = new List<TIdentifier>();
        }

        public SpawnableObjectCell(Transform newTransform, TIdentifier[] newIds)
        {
            transform = newTransform;
            ids = new List<TIdentifier>(newIds);
        }
    }
}