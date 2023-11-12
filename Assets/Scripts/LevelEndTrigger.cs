using UnityEngine;

public class LevelEndTrigger : MonoBehaviour
{
    private LevelEnd _levelEnd;
    
    private void Start()
    {
        _levelEnd = FindObjectOfType<LevelEnd>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        
        _levelEnd.SwitchLevel();
    }

    private void OnDestroy()
    {
        Time.timeScale = 1;
    }
}