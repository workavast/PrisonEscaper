using UnityEngine;

public class LevelEndTrigger : MonoBehaviour
{
    private bool _isTriggered;
    private LevelEnd _levelEnd;
    
    private void Start()
    {
        _levelEnd = FindObjectOfType<LevelEnd>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isTriggered)
        {
            Debug.LogWarning($"Second enter. Maybe your player have two colliders: trigger and non trigger");
            return;
        }
        
        if (!other.CompareTag("Player"))
            return;

        _isTriggered = true;
        _levelEnd.SwitchLevel();
    }

    private void OnDestroy()
    {
        Time.timeScale = 1;
    }
}