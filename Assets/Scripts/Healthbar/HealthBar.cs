using UnityEngine;
using UnityEngine.UI;

namespace New_Folder.Healthbar
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Slider healthBar_slider;

        private void Start()
        {
            healthBar_slider = GetComponent<Slider>();
        }

        public void SetMaxHealth(float health)
        {
            healthBar_slider.maxValue = health;
            healthBar_slider.value = health;
        }

        public void SetHealth(float health)
        {
            healthBar_slider.value = health;
        }
    }
}
