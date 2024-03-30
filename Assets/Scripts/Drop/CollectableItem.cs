using System.Collections;
using Core;
using PlayerInventory;
using PlayerInventory.Scriptable;
using SerializableDictionaryExtension;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(AudioSource))]
public class CollectableItem : MonoBehaviour, IInteractive
{
    [SerializeField] private GameObject interactKeyImg;
    [SerializeField] private SpriteRenderer itemSpriteRenderer;
    [SerializeField] private Animator itemAnimator;
    [SerializeField] private SerializableDictionary<ItemRarity, GameObject> glowPrefabs;

    [field: SerializeField] public bool Interactable { get; private set; } = true;
   
    private AudioSource _source;
    private VisualEffect _vfx;
    private Item _item;
    
    public Item Item
    {
        get => _item;
        set
        {
            _item = value;
            itemSpriteRenderer.sprite = _item.Sprite;
            
            if (glowPrefabs.ContainsKey(_item.Rarity))
                _vfx = Instantiate(glowPrefabs[_item.Rarity], transform).GetComponent<VisualEffect>();
        }
    }

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
        _source.Stop();
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

    public void Interact() => PickUpItem();
    
    private void PickUpItem()
    {
        if (Inventory.HasBagEmptySlot())
        {
            itemAnimator.Play("Picked");
            _source.Play();
            Interactable = false;
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