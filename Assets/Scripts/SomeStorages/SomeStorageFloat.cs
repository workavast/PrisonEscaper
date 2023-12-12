using System;
using UnityEngine;

namespace SomeStorages
{
    [Serializable]
    public class SomeStorageFloat : SomeStorageBase<float>
    {
        public override float FillingPercentage => (currentValue / maxValue);
        public override bool IsFull => (currentValue >= maxValue);
        public override bool IsEmpty => (currentValue <= minValue);

        public override event Action OnChange;
        public override event Action<float> OnMaxValueChange;
        public override event Action<float> OnCurrentValueChange;
        public override event Action<float> OnMinValueChange;
    
        public SomeStorageFloat()
        {
            maxValue = 0;
            currentValue = 0;
            minValue = 0;
        }

        public SomeStorageFloat(float maxValue)
        {
            this.maxValue = maxValue;
            currentValue = 0;
            minValue = maxValue > 0 ? 0 : float.MinValue;
        }

        public SomeStorageFloat(float maxValue, float currentValue)
        {
            this.maxValue = maxValue;
            this.currentValue = currentValue;
            minValue = maxValue > 0 ? 0 : float.MinValue;
        }

        public SomeStorageFloat(float maxValue, float currentValue, float minValue)
        {
            this.maxValue = maxValue;
            this.currentValue = currentValue;
            this.minValue = minValue;
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