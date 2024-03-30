using System.Collections.Generic;
using GameCycleFramework;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace GameCode.Light
{
    public class GlobalLampBlinks : MonoBehaviour, IGameCycleUpdate
    {
        public static GlobalLampBlinks Instance { get; private set; }
        
        [SerializeField] [Range(0,1)] private float frequency;
        [SerializeField] [Range(0,1)] private float minLampBlinkPercent;
        [SerializeField] [Range(0,1)] private float maxLampBlinkPercent;

        [Inject] private IGameCycleController _gameCycleController; 
        
        private List<BlinkingLamp> _lamps = new();
    
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }
    
            Instance = this;
            
            _gameCycleController.AddListener(GameCycleState.Gameplay, this);
        }
    
        public void GameCycleUpdate()
        {
            if (Random.value < frequency * Time.deltaTime)
                Blink();
        }
    
        public void AddLamp(BlinkingLamp newBlinkingLamp)
        {
            _lamps.Add(newBlinkingLamp);
            newBlinkingLamp.OnDestroyEvent += RemoveLamp;
        }
        
        public void Clear()
            => _lamps = new List<BlinkingLamp>();
        
        private void Blink()
        {
            int randomLampCount = Random.Range((int)(_lamps.Count * minLampBlinkPercent), (int)(_lamps.Count * maxLampBlinkPercent));
    
            List<int> blinkLamps = new List<int>();
            for (int i = 0; i < randomLampCount; i++)
            {
                int lampIndex = Random.Range(0, _lamps.Count);
                if (blinkLamps.Contains(lampIndex))
                {
                    i--;
                    continue;
                }
    
                _lamps[lampIndex].Blink();
                blinkLamps.Add(lampIndex);
            }
        }
        
        private void RemoveLamp(BlinkingLamp blinkingLamp)
            => _lamps.Remove(blinkingLamp);
    }
}
