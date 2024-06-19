using Core;
using UnityEngine;

public class LevelEndTrigger : MonoBehaviour, IInteractive
{
    [SerializeField] private GameObject interactKeyImg;
    
    public bool Interactable { get; private set; } = true;
    private LevelEnd _levelEnd;

    private void Awake()
    {
        interactKeyImg.SetActive(false);
    }

    private void Start()
    {
        _levelEnd = FindObjectOfType<LevelEnd>();
    }
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player") && Interactable) 
            interactKeyImg.SetActive(true);
    }
    
    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player")) 
            interactKeyImg.SetActive(false);
    }
    
    public void Interact()
    {
        Interactable = false;
        interactKeyImg.SetActive(false);
        _levelEnd.SwitchLevel();
    }
}