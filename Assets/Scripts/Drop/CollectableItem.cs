using System.Collections;
using GameCode.Core;
using GameCode.Drop;
using PlayerInventory;
using PlayerInventory.Scriptable;
using SerializableDictionaryExtension;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CollectableItem : MonoBehaviour
{
    [SerializeField] private Transform itemTransform;
    [SerializeField] private GameObject interactKeyImg;
    [SerializeField] private SpriteRenderer itemSpriteRenderer;
    [SerializeField] private Animator itemAnimator;
    [SerializeField] private TriggerZone2D triggerZone2D;
    [SerializeField] private InteractionZone interactionZone;
    [SerializeField] private SerializableDictionary<ItemRarity, CollectableItemVfx> glowPrefabs;

    public bool Interactable => interactionZone.Interactable;
   
    private AudioSource _source;
    private CollectableItemVfx _vfx;
    private Item _item;
    
    public Item Item
    {
        get => _item;
        set
        {
            _item = value;
            itemSpriteRenderer.sprite = _item.Sprite;
            
            if (glowPrefabs.ContainsKey(_item.Rarity))
                _vfx = Instantiate(glowPrefabs[_item.Rarity], itemTransform);
        }
    }

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
        _source.Stop();

        interactionZone.OnInteract += PickUpItem;
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

    private void PickUpItem()
    {
        if (Inventory.HasBagEmptySlot())
        {
            itemAnimator.Play("Picked");
            _vfx.ActivatePickUpAnimation();
            _source.Play();
            interactionZone.SetInteractionState(false);
            interactKeyImg.SetActive(false);
            Inventory.AddItem(_item);
            StartCoroutine(OnPickedUp());
        }
    }
    
    private IEnumerator OnPickedUp() 
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}