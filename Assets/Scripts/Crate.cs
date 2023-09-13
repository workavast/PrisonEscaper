using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ItemDropper))]
public class Crate : MonoBehaviour
{
    [SerializeField] private GameObject interactKeyImg;
    [SerializeField] private Sprite brokenCrate;
    [SerializeField] SpriteRenderer spriteRenderer;
    
    private ItemDropper _dropper;
    private bool _hasDrop = false;
    private void Awake()
    {
        interactKeyImg.SetActive(false);
        _dropper = GetComponent<ItemDropper>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player") && !_hasDrop) 
            interactKeyImg.SetActive(true);
        
    }
    
    private void OnTriggerStay2D(Collider2D col)
    {
        if (!_hasDrop && col.CompareTag("Player") && Input.GetKey(KeyCode.F))
        {
            spriteRenderer.sprite = brokenCrate;
            _dropper.DropItems();
            _hasDrop = true;
            interactKeyImg.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player")) interactKeyImg.SetActive(false);
    }
}