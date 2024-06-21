using GameCode.Core;
using UnityEngine;

[RequireComponent(typeof(ItemDropper))]
[RequireComponent(typeof(AudioSource))]
public class Crate : MonoBehaviour
{
    [SerializeField] private GameObject interactKeyImg;
    [SerializeField] private Sprite brokenCrate;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private TriggerZone2D triggerZone2D;
    [SerializeField] private InteractionZone interactionZone;

    private bool Interactable => interactionZone.Interactable;
    private ItemDropper _dropper;
    
    private void Awake()
    {
        interactKeyImg.SetActive(false);
        _dropper = GetComponent<ItemDropper>();

        interactionZone.OnInteract += OpenCrate;
        triggerZone2D.OnTriggerEnter2DEvent += TriggerEnter2D;
        triggerZone2D.OnTriggerExit2DEvent += TriggerExit2D;
    }

    private void TriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player") && Interactable) 
            interactKeyImg.SetActive(true);
    }

    private void TriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player")) 
            interactKeyImg.SetActive(false);
    }
    
    private void OpenCrate()
    {
        interactionZone.SetInteractionState(false);
        spriteRenderer.sprite = brokenCrate;
        _dropper.DropItems();
        interactKeyImg.SetActive(false);
        audioSource.Play();
    } 
}