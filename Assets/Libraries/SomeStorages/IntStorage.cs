using System;
using UnityEngine;

namespace SomeStorages
{
    [System.Serializable]
    public class IntStorage : SomeStorageBase<int>
    {
        public override float FillingPercentage => (float)currentValue / (float)maxValue;
        public override bool IsFull => currentValue >= maxValue;
        public override bool IsEmpty => currentValue <= minValue;
        
        public override event Action OnChange;
        public override event Action<int> OnMaxValueChange;
        public override event Action<int> OnCurrentValueChange;
        public override event Action<int> OnMinValueChange;
        
        public IntStorage(int maxValue = 0, int currentValue = 0, int minValue = 0)
        {
            this.maxValue = maxValue;
            this.currentValue = currentValue;
            this.minValue = minValue;
            
#if UNITY_EDITOR
            if(this.currentValue > this.maxValue)
                Debug.LogWarning("Attention!: current value greater then max value");
            if(this.minValue > this.maxValue)
                Debug.LogWarning("Attention!: minimal value greater then max value");
            if(this.minValue > this.currentValue)
                Debug.LogWarning("Attention!: minimal value greater then current value");
#endif
        }

        public override void SetMaxValue(int newMaxValue)
        {
            maxValue = newMaxValue;
            currentValue = Mathf.Clamp(currentValue, minValue, maxValue);
            OnMaxValueChange?.Invoke(maxValue);
            OnChange?.Invoke();
        }

        public override void SetCurrentValue(int newCurrentValue)
        {
            currentValue = Mathf.Clamp(newCurrentValue, minValue, maxValue);
            OnCurrentValueChange?.Invoke(currentValue);
            OnChange?.Invoke();
        }

        public override void SetMinValue(int newMinValue)
        {
            minValue = newMinValue;
            currentValue = Mathf.Clamp(currentValue, minValue, maxValue);
            OnMinValueChange?.Invoke(minValue);
            OnChange?.Invoke();
        }

        public override void ChangeCurrentValue(int value)
        {
            currentValue = Mathf.Clamp(currentValue + value, minValue, maxValue);
            OnCurrentValueChange?.Invoke(currentValue);
            OnChange?.Invoke();
        }
    }
}