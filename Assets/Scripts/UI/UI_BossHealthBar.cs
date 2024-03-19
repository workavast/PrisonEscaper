using Enemies;
using GameCycleFramework;
using LevelGeneration.LevelsGenerators;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class UI_BossHealthBar : MonoBehaviour, IIniteableUI , IGameCycleExit
{
    [SerializeField] private Slider healthSlider;

    [Inject] private readonly IGameCycleController _gameCycleController;
    [Inject] private readonly LevelGeneratorBase _levelGenerator;
    
    private BossMutantArena _boss;
    private UI_SomeBar _healthBar;

    public void Init()
    {
        _gameCycleController.AddListener(GameCycleState.LocationGeneration, this);
        SetDisable();
    }

    public void GameCycleExit()
    {
        var boss = FindObjectOfType<BossMutantArena>();

        if (boss != null)
            SetBoss(boss);
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
