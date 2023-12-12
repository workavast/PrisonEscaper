using UnityEngine;
using UnityEngine.UI;

public class UI_BossHealthBar : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    
    private BossMutantArena _boss;
    private UI_SomeBar _healthBar;

    private void Start()
    {
        var boss = FindObjectOfType<BossMutantArena>();
        
        if(boss != null)
            SetBoss(boss);
        SetDisable();
    }

    private void SetBoss(BossMutantArena newBoss)
    {
        _boss = newBoss;
        
        _healthBar = new UI_SomeBar(this, healthSlider, _boss.StatsSystem.Health.FillingPercentage);
        _boss.OnStartBossBattle += SetActive;
        _boss.OnBossDie += SetDisable;
        _boss.StatsSystem.Health.OnChange += UpdateBar;
    }
    
    private void UpdateBar() => _healthBar.SetTargetValue(_boss.StatsSystem.Health.FillingPercentage);

    private void SetActive() => gameObject.SetActive(true);

    private void SetDisable() => gameObject.SetActive(false);
}
