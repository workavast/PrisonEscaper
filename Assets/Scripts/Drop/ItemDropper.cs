using GameCode.Drop;
using Unity.Mathematics;
using UnityEngine;

public class ItemDropper : MonoBehaviour
{
    [SerializeField] private Vector3 dropPositionOffset;
    [SerializeField] private CollectableItem collectablePrefab;
    [Space]
    [SerializeField] private DropListConfig dropList;

    private const float DropRange = 2f;

    public void DropItems()
    {
        foreach (var item in dropList.DropSet.GetDropSet())
        {
            var position = transform.position + Vector3.right * UnityEngine.Random.Range(0, DropRange);
            var droppedObject = Instantiate(collectablePrefab, position + dropPositionOffset, quaternion.identity);
            droppedObject.Item = item;
        }    
    }
}
