using System;
using UnityEngine;

namespace SomeStorages
{
    [Serializable]
    public class FloatStorage : SomeStorageBase<float>
    {
        public override float FillingPercentage => (currentValue / maxValue);
        public override bool IsFull => (currentValue >= maxValue);
        public override bool IsEmpty => (currentValue <= minValue);

        public override event Action OnChange;
        public override event Action<float> OnMaxValueChange;
        public override event Action<float> OnCurrentValueChange;
        public override event Action<float> OnMinValueChange;
        
        public FloatStorage(float maxValue = 0, float currentValue = 0, float minValue = 0)
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
        
        public override void SetMaxValue(float newMaxValue)
        {
            maxValue = newMaxValue;
            currentValue = Mathf.Clamp(currentValue, minValue, maxValue);
            OnMaxValueChange?.Invoke(maxValue);
            OnChange?.Invoke();
        }

        public override void SetCurrentValue(float newCurrentValue)
        {
            currentValue = Mathf.Clamp(newCurrentValue, minValue, maxValue);
            OnCurrentValueChange?.Invoke(currentValue);
            OnChange?.Invoke();
        }

        public override void SetMinValue(float newMinValue)
        {
            minValue = newMinValue;
            currentValue = Mathf.Clamp(currentValue, minValue, maxValue);
            OnMinValueChange?.Invoke(minValue);
            OnChange?.Invoke();
        }
    
        public override void ChangeCurrentValue(float value)
        {
            currentValue = Mathf.Clamp(currentValue + value, minValue, maxValue);
            OnCurrentValueChange?.Invoke(currentValue);
            OnChange?.Invoke();
        }
    }
}