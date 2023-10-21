using UnityEngine;

namespace LevelGeneration
{
    public interface IBlockDataForGUI
    {
        public GameObject gameObject { get; }

        public void SetAllPoints();
        public void SetEnemiesPoints();
        public void SetLootBoxPoints();
        public void SetTrapsPoints();

        public void ReSetAllPoints();
        public void ReSetEnemiesPoints();
        public void ReSetLootBoxPoints();
        public void ReSetTrapsPoints();

        public void SetSizePoints();
    }
}