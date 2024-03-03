using System;
using UnityEngine;

namespace CustomTimer
{
    public class Timer
    {
        public float CurrentTime { get; private set; }
        public float MaxTime { get; private set; }
        
        public bool TimerIsEnd { get; private set; }
        public bool Paused { get; private set; }
        
        public event Action OnTimerEnd;
        
        public Timer(float maxValue, float startValue = 0, bool paused = false)
        {
            MaxTime = maxValue;
            CurrentTime = startValue;
            Paused = paused;
            
            if (CurrentTime >= MaxTime)
                TimerIsEnd = true;
        }
        
        public void SetMaxValue(float newMaxValue, bool saveCurrentValue = false)
        {
            MaxTime = newMaxValue;
            if (saveCurrentValue)
                CurrentTime = Mathf.Clamp(CurrentTime, 0, MaxTime);
            else
                CurrentTime = 0;
        }

        public void Reset(bool paused = false)
        {
            TimerIsEnd = false;
            CurrentTime = 0;
            Paused = paused;
        }
        
        public void SetPause() => Paused = true;
        
        public void Continue() => Paused = false;
        
        public void Tick(float time)
        {
            if(TimerIsEnd || Paused) return;
            
            UpdateTimer(time);
        }

        private void UpdateTimer(float time)
        {
            CurrentTime += time;
            CurrentTime = Mathf.Clamp(CurrentTime, 0, MaxTime);
            
            if (CurrentTime >= MaxTime)
            {
                TimerIsEnd = true;
                OnTimerEnd?.Invoke();
            }
        }
    }
}