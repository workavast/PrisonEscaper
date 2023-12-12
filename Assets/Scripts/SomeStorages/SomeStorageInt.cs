using System;
using UnityEngine;

namespace SomeStorages
{
    [System.Serializable]
    public class SomeStorageInt : SomeStorageBase<int>
    {
        public override float FillingPercentage => (float)currentValue / (float)maxValue;
        public override bool IsFull => currentValue >= maxValue;
        public override bool IsEmpty => currentValue <= minValue;
        
        public override event Action OnChange;
        public override event Action<int> OnMaxValueChange;
        public override event Action<int> OnCurrentValueChange;
        public override event Action<int> OnMinValueChange;

        public SomeStorageInt()
        {
            maxValue = 0;
            currentValue = 0;
            minValue = 0;
        }

        public SomeStorageInt(int maxValue)
        {
            this.maxValue = maxValue;
            currentValue = 0;
            minValue = maxValue > 0 ? 0 : int.MinValue;
        }

        public SomeStorageInt(int maxValue, int currentValue)
        {
            this.maxValue = maxValue;
            this.currentValue = currentValue;
            minValue = maxValue > 0 ? 0 : int.MinValue;
        }

        public SomeStorageInt(int maxValue, int currentValue, int minValue)
        {
            this.maxValue = maxValue;
            this.currentValue = currentValue;
            this.minValue = minValue;
        }

        public override void SetMaxValue(int newMaxValue)
        {
            maxValue = newMaxValue;
            currentValue = Mathf.Clamp(currentValue, minValue, maxValue);
            OnMaxValueChange?.Invoke(maxValue);
        }

        public override void SetCurrentValue(int newCurrentValue)
        {
            currentValue = Mathf.Clamp(newCurrentValue, minValue, maxValue);
            OnCurrentValueChange?.Invoke(currentValue);
        }

        public override void SetMinValue(int newMinValue)
        {
            minValue = newMinValue;
            currentValue = Mathf.Clamp(currentValue, minValue, maxValue);
            OnMinValueChange?.Invoke(minValue);
        }

        public override void ChangeCurrentValue(int value)
        {
            currentValue = Mathf.Clamp(currentValue + value, minValue, maxValue);
            OnCurrentValueChange?.Invoke(currentValue);
        }
    }
}