using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ItemDropper))]
public class Crate : MonoBehaviour, IInteractive
{
    [SerializeField] private GameObject interactKeyImg;
    [SerializeField] private Sprite brokenCrate;
    [SerializeField] SpriteRenderer spriteRenderer;

    public bool Interactable { get; private set; } = true;
    private ItemDropper _dropper;
    
    private void Awake()
    {
        interactKeyImg.SetActive(false);
        _dropper = GetComponent<ItemDropper>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player") && Interactable) 
            interactKeyImg.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player")) interactKeyImg.SetActive(false);
    }
    
    public void Interact() => OpenCrate();

    private void OpenCrate()
    {
        spriteRenderer.sprite = brokenCrate;
        _dropper.DropItems();
        Interactable = false;
        interactKeyImg.SetActive(false);
    } 
}