using System;
using UnityEngine;

namespace SomeStorages
{
    [System.Serializable]
    public abstract class SomeStorageBase<TDataType> : IReadOnlySomeStorage<TDataType> where TDataType : struct
    {
        [SerializeField] protected TDataType maxValue;
        [SerializeField] protected TDataType currentValue;
        [SerializeField] protected TDataType minValue;

        public TDataType MaxValue => maxValue;
        public TDataType CurrentValue => currentValue;
        public TDataType MinValue => minValue;

        public abstract float FillingPercentage { get; }
        public abstract bool IsFull { get; }
        public abstract bool IsEmpty { get; }

        public abstract event Action OnChange;
        public abstract event Action<TDataType> OnMaxValueChange;
        public abstract event Action<TDataType> OnCurrentValueChange;
        public abstract event Action<TDataType> OnMinValueChange;

        public abstract void SetMaxValue(TDataType newMaxValue);

        public abstract void SetCurrentValue(TDataType newCurrentValue);

        public abstract void SetMinValue(TDataType newMinValue);

        public abstract void ChangeCurrentValue(TDataType value);
    }
}