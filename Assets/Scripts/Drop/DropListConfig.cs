using PlayerInventory.Scriptable;
using UnityEngine;

namespace GameCode.Drop
{
    [CreateAssetMenu(fileName = nameof(DropListConfig), menuName = "Configs/" + nameof(DropListConfig))]
    public class DropListConfig : ScriptableObject
    {
        [SerializeField] private WeightDrop dropSet;
        [Space]
        [Header("Auto filling (use context menu: Fill Random Drop or Fill Const Drop)")]
        [SerializeField] private int startWeight;
        [SerializeField] private int step;
        [SerializeField] private Item[] items;
        
        public WeightDrop DropSet => dropSet;

        [ContextMenu("Fill Random Drop")]
        private void FillRandomDrop() 
            => dropSet.FillRandomDrop(startWeight, step, items);

        [ContextMenu("Fill Const Drop")]
        private void AutoFillConst() 
            => dropSet.FillConstDrop(items);
    }
}