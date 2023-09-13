using System;
using System.Collections.Generic;
using PlayerInventory.Scriptable;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[Serializable]
public class WeightDrop
{
    
    [Range(0,10)]
    [SerializeField] private int randomDropAmount;
    [SerializeField] private WeightedItem[] randomDropList;
    [SerializeField] private Item[] constantDropList;

    private int[] _weights;
    private int _maxWeight = 0;

    private void InitTotalWeight()
    {
        _weights = new int[randomDropList.Length];

        var i = 0;

        foreach (var item in randomDropList)
        {
            if(_maxWeight < item.weight)
                _maxWeight = item.weight;
            
            _weights[i++] = item.weight;
        }
    }

    public Item[] GetDropSet()
    {
        InitTotalWeight();
        var dropSet = new List<Item>();

        if (randomDropList.Length > 0)
        {
            for (var i = 0; i < randomDropAmount; i++)
            {
                var rand = Random.Range(0, _maxWeight);
                
                if (rand < _weights[0])
                    continue;
                
                var j = 0;
                while (_weights[j] < rand)
                    j++;
            
                if (randomDropList[j].item is null)
                    continue;
            
                dropSet.Add(randomDropList[j].item);
            }  
        }


        foreach (var item in constantDropList)
        {
            dropSet.Add(item);
        }

        return dropSet.ToArray();
    }
}