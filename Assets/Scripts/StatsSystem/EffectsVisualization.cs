using SerializableDictionaryExtension;
using UnityEngine;

namespace GameCode.StatsSystem
{
    public class EffectsVisualization : MonoBehaviour
    {
        [SerializeField] private SerializableDictionary<StatusEffect, GameObject> statusParticles;

        public void SetActiveEffect(StatusEffect statusEffect, bool active)
        {
            if(statusParticles.ContainsKey(statusEffect))
                statusParticles[statusEffect].SetActive(active);
        }
    }
}